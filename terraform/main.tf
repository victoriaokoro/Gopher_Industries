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

  project = "sit-23t2-food-remedy-7f9b4c0"
  region  = "australia-southeast2"
}

resource "google_storage_bucket" "example" {
  name     = "foodremedy-api-example"
  location = "AUSTRALIA-SOUTHEAST2"

}