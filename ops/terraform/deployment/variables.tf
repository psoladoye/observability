variable "region" {
  default = "us-central1"
}

variable "zone" {
  default = "us-central1-c"
}

variable "project" {}

variable "master_authorized_networks" {}

variable "stack_name" {}

variable "application_namespace" {
  default = "default"
}

variable "observability_namespace" {
  default = "observability"
}