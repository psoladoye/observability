provider "google" {
  project = var.project
  region  = var.region
  zone    = var.zone
}

provider "google-beta" {
  project = var.project
  region  = var.region
  zone    = var.zone
}

data "google_project" "project" {}

resource "random_id" "stack" {
  byte_length = 4
}
