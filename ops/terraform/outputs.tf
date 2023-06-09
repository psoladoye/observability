output "cluster_name" {
  value = module.deployment.cluster_name
}

output "deployment_service_account" {
  value = module.deployment.cluster_service_account
}

output "application_namespace" {
  value = module.deployment.application_namespace
}

output "observability_namespace" {
  value = module.deployment.observability_namespace
}
