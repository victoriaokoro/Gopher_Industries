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

resource "google_artifact_registry_repository" "foodremedy_backend" {
  repository_id = "${var.env}-foodremedy-backend"
  format = "DOCKER"
  location = var.region
  project = var.project
}

resource "google_artifact_registry_repository" "foodremedy_database" {
  repository_id = "${var.env}-foodremedy-database"
  format = "DOCKER"
  location = var.region
  project = var.project
}