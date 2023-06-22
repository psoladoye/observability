variable "region" {
  default = "us-central1"
}

variable "zone" {
  default = "us-central1-c"
}

variable "project" {
  default = ""
}

variable "master_authorized_networks" {}

variable "stack_name" {
  type = string
}