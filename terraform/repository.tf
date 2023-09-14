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


resource "google_artifact_registry_repository" "foodremedy_frontend" {
  repository_id = "${var.env}-foodremedy-frontend"
  format        = "DOCKER"
  location      = var.region
  project       = var.project
}