output "cluster_name" {
  value = module.deployment.cluster_name
}

output "node_service_account" {
  value = module.deployment.node_service_account
}

output "k8s_service_account" {
  value = module.deployment.k8s_service_account
}

output "k8s_service_account_name" {
  value = module.deployment.k8s_service_account_name
}

output "application_namespace" {
  value = module.deployment.application_namespace
}

output "observability_namespace" {
  value = module.deployment.observability_namespace
}

output "gcp_project" {
  value = var.project
}

output "gcp_zone" {
  value = var.zone
}
