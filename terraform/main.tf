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

