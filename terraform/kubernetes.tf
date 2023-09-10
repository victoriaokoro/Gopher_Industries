resource "kubernetes_namespace" "foodremedy_namespace" {
  metadata {
    name = "${var.env}-foodremedy"
  }
}

resource "kubernetes_deployment" "foodremedy_ApiDeployment" {
  metadata {
    name      = "${var.env}-foodremedy-api"
    namespace = kubernetes_namespace.foodremedy_namespace.metadata[0].name
  }

  spec {
    replicas = 1

    selector {
      match_labels = {
        app = "${var.env}-foodremedy-api"
      }
    }

    template {
      metadata {
        labels = {
          app = "${var.env}-foodremedy-api"
        }
      }

      spec {
        container {
          name  = "${var.env}-foodremedy-api"
          image = "your-dotnet-core-image:tag" // TODO what image? 
          port {
            name = "http"
            container_port = 80
          }
        }
      }
    }
  }
}

resource "kubernetes_service" "foodremedy_ApiService" {
  metadata {
    name      = "${var.env}-foodremedy-api"
    namespace = kubernetes_namespace.foodremedy_namespace.metadata[0].name
  }

  spec {
    selector = {
      app = "${var.env}-foodremedy-api"
    }

    port {
      name       = "http"
      port       = 80
      target_port = 80
    }

    type = "LoadBalancer"
  }
}

resource "kubernetes_deployment" "foodremedy_DatabaseDeployment" {
  metadata {
    name      = "${var.env}-foodremedy-db"
    namespace = kubernetes_namespace.foodremedy_namespace.metadata[0].name
  }

  spec {
    replicas = 1

    selector {
      match_labels = {
        app = "${var.env}-foodremedy-db"
      }
    }

    template {
      metadata {
        labels = {
          app = "${var.env}-foodremedy-db"
        }
      }

      spec {
        container {
          name  = "${var.env}-foodremedy-db"
          image = "mysql:latest"

          env {
            name  = "MYSQL_ROOT_PASSWORD"
            value = "your-root-password"
          }

          port {
            name = "database"
            container_port = 3306
          }
        }
      }
    }
  }
}

resource "kubernetes_service" "foodremedy_DatabaseService" {
  metadata {
    name      = "${var.env}-foodremedy-db"
    namespace = kubernetes_namespace.foodremedy_namespace.metadata[0].name
  }

  spec {
    selector = {
      app = "${var.env}-foodremedy-db"
    }

    port {
      name       = "db"
      port       = 3306
      target_port = 3306
    }
  }
}
