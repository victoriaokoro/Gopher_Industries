terraform {
  required_providers {
    google = {
      source  = "hashicorp/google"
      version = "4.51.0"
    }
  }

  backend "gcs" {
    bucket = "foodremedy-api-tfstate"
    prefix = "env/dev"
  }
}

provider "google" {
  credentials = file("credentials.json")

  project = var.project
  region  = var.region
}

provider "kubernetes" {

}

resource "google_artifact_registry_repository" "foodremedy_backend" {
  repository_id = "${var.env}-foodremedy-backend"
  format        = "DOCKER"
  location      = var.region
  project       = var.project
}

resource "google_artifact_registry_repository" "foodremedy_database" {
  repository_id = "${var.env}-foodremedy-database"
  format        = "DOCKER"
  location      = var.region
  project       = var.project
}

// GKE configuration
resource "google_container_cluster" "gke_cluster" {
  name               = "${var.env}-gke-cluster"
  location           = var.region
  initial_node_count = 1

  node_config {
    machine_type = "n1-standard-2"
  }
}

resource "kubernetes_namespace" "foodremedy" {
  metadata {
    name = "${var.env}-app"
  }
}

resource "kubernetes_deployment" "foodremedy_deployment" {
  metadata {
    name      = "${var.env}-app"
    namespace = kubernetes_namespace.foodremedy.metadata[0].name
  }

  spec {
    replicas = 1

    selector {
      match_labels = {
        app = "${var.env}-app"
      }
    }

    template {
      metadata {
        labels = {
          app = "${var.env}-app"
        }
      }

      spec {
        container {
          name  = "${var.env}-app"
          image = "your-docker-image:tag"
        }
      }
    }
  }
}

resource "kubernetes_service" "app_service" {
  metadata {
    name      = "${var.env}-app"
    namespace = kubernetes_namespace.foodremedy.metadata[0].name
  }

  spec {
    selector = {
      app = "${var.env}-app"
    }

    port {
      name         = "http"
      port         = 80
      target_port = 80
    }

    type = "LoadBalancer"
  }
}

resource "kubernetes_stateful_set" "db_stateful_set" {
  metadata {
    name      = "${var.env}-db"
    namespace = kubernetes_namespace.foodremedy.metadata[0].name
  }

  spec {
    service_name = kubernetes_service.db_service.id
    replicas = 1

    selector {
      match_labels = {
        app = "${var.env}-db"
      }
    }

    template {
      metadata {
        labels = {
          app = "${var.env}-db"
        }
      }

      spec {
        container {
          name  = "${var.env}-db"
          image = "your-database-image:tag"
        }
      }
    }
  }
}

resource "kubernetes_service" "db_service" {
  metadata {
    name      = "${var.env}-db"
    namespace = kubernetes_namespace.foodremedy.metadata[0].name
  }

  spec {
    selector = {
      app = "${var.env}-db"
    }

    port {
      name       = "db"
      port       = 27017
      target_port = 27017
    }
  }
}

resource "google_compute_global_forwarding_rule" "app_lb" {
  name       = "${var.env}-app-lb"
  target     = kubernetes_service.app_service.status[0].load_balancer.ingress[0].ip
  port_range = "80"
}

resource "google_compute_target_http_proxy" "app_lb_proxy" {
  name    = "${var.env}-app-lb-proxy"
  url_map = google_compute_url_map.app_lb_url_map.self_link
}

resource "google_compute_url_map" "app_lb_url_map" {
  name            = "${var.env}-app-lb-url-map"
  default_service = kubernetes_service.app_service.metadata[0].self_link

  host_rule {
    hosts = ["your-domain.com"]
  }

  path_matcher {
    name        = "all"
    default_url = kubernetes_service.app_service.metadata[0].self_link
  }
}
