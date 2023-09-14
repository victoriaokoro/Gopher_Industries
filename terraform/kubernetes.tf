resource "kubernetes_namespace" "foodremedy_namespace" {
  metadata {
    name = local.foodremedy_app_name
  }
}

resource "kubernetes_deployment" "foodremedy_ApiDeployment" {
  metadata {
    name      = local.foodremedy_backend_name
    namespace = kubernetes_namespace.foodremedy_namespace.metadata[0].name
  }

  spec {
    replicas = 1

    selector {
      match_labels = {
        app = local.foodremedy_backend_name
      }
    }

    template {
      metadata {
        labels = {
          app = local.foodremedy_backend_name
        }
      }

      spec {
        container {
          name  = local.foodremedy_backend_name
          image = "${google_artifact_registry_repository.foodremedy_backend.name}:latest"
          port {
            name           = "http"
            container_port = 80
          }
        }
      }
    }
  }
}

resource "kubernetes_service" "foodremedy_ApiService" {
  metadata {
    name      = local.foodremedy_backend_name
    namespace = kubernetes_namespace.foodremedy_namespace.metadata[0].name
  }

  spec {
    selector = {
      app = local.foodremedy_backend_name
    }

    port {
      name        = "http"
      port        = 80
      target_port = 80
    }

    type = "LoadBalancer"
  }
}

resource "kubernetes_deployment" "foodremedy_DatabaseDeployment" {
  metadata {
    name      = local.foodremedy_database_name
    namespace = kubernetes_namespace.foodremedy_namespace.metadata[0].name
  }

  spec {
    replicas = 1

    selector {
      match_labels = {
        app = local.foodremedy_database_name
      }
    }

    template {
      metadata {
        labels = {
          app = local.foodremedy_database_name
        }
      }

      spec {
        container {
          name  = local.foodremedy_database_name
          image = "${google_artifact_registry_repository.foodremedy_database.name}:latest"

          env {
            name  = "MYSQL_ROOT_PASSWORD"
            value = var.foodremedy_database_root_password
          }

          port {
            name           = "database"
            container_port = 3306
          }
        }
      }
    }
  }
}

resource "kubernetes_service" "foodremedy_DatabaseService" {
  metadata {
    name      = local.foodremedy_database_name
    namespace = kubernetes_namespace.foodremedy_namespace.metadata[0].name
  }

  spec {
    selector = {
      app = local.foodremedy_database_name
    }

    port {
      name        = "db"
      port        = 3306
      target_port = 3306
    }
  }
}


resource "kubernetes_deployment" "foodremedy_FrontendDeployment" {
  metadata {
    name      = local.foodremedy_frontend_name
    namespace = kubernetes_namespace.foodremedy_namespace.metadata[0].name
  }

  spec {
    replicas = 1

    selector {
      match_labels = {
        app = local.foodremedy_frontend_name
      }
    }

    template {
      metadata {
        labels = {
          app = local.foodremedy_frontend_name
        }
      }

      spec {
        container {
          name  = local.foodremedy_frontend_name
          image = "${google_artifact_registry_repository.foodremedy_frontend.name}:latest"
          port {
            name           = "frontend"
            container_port = 3000
          }
        }
      }
    }
  }
}

resource "kubernetes_service" "foodremedy_FrontendService" {
  metadata {
    name      = local.foodremedy_frontend_name
    namespace = kubernetes_namespace.foodremedy_namespace.metadata[0].name
  }

  spec {
    selector = {
      app = local.foodremedy_frontend_name
    }

    port {
      name          = "http"
      port          = 80
      target_port = 80
    }

    type = "LoadBalancer"
  }
}