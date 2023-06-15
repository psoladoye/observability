output "application_namespace" {
  value = local.application_namespace
}

output "observability_namespace" {
  value = local.observability_namespace
}

output "node_service_account" {
  value = module.gke.service_account
}

output "cluster_name" {
  value = local.gke_cluster_name
}

output "k8s_app_service_account" {
  value = module.app_workload_identity.gcp_service_account_email
}

output "k8s_app_service_account_name" {
  value = module.app_workload_identity.k8s_service_account_name
}

output "k8s_observability_service_account" {
  value = module.observability_workload_identity.gcp_service_account_email
}

output "k8s_observability_service_account_name" {
  value = module.observability_workload_identity.k8s_service_account_name
}