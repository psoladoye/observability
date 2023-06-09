output "application_namespace" {
  value = local.application_namespace
}

output "observability_namespace" {
  value = local.observability_namespace
}

output "cluster_service_account" {
  value = module.gke.service_account
}

output "cluster_name" {
  value = local.gke_cluster_name
}