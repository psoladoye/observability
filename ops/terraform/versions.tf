terraform {
  required_version = ">= 1.4.6"

  backend "gcs" {
    bucket = "tf-state-dev-007"
    prefix = "terraform/state-9"
  }

  required_providers {
    google = {
      source  = "hashicorp/google"
      version = "<= 4.68.0"
    }
    google-beta = {
      source  = "hashicorp/google-beta"
      version = "<= 4.68.0"
    }
    kubernetes = {
      source  = "hashicorp/kubernetes"
      version = "<= 26.1.1"
    }
  }
}