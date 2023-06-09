module "environment" {
  source      = "./environment"
  environment = local.environment
  project     = var.project
  region      = var.region
  zone        = var.zone
}

module "deployment" {
  source                     = "./deployment"
  project                    = var.project
  region                     = var.region
  zone                       = var.zone
  master_authorized_networks = var.authorized_networks
  stack_name                 = module.environment.stack_name
  application_namespace      = var.application_namespace
  observability_namespace    = var.observability_namespace
}

