variable "region" {
  default = "us-central1"
}

variable "zone" {
  default = "us-central1-c"
}

variable "project" {}

variable "authorized_networks" {
  description = "The cidr blocks authorized to manage the kubernetes control plane."
  type = set(object({
    cidr_block   = string
    display_name = string
  }))
  default = []
}