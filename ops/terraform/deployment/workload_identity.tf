module "app_workload_identity" {
  source     = "terraform-google-modules/kubernetes-engine/google//modules/workload-identity"
  name       = "${var.stack_name}-app"
  namespace  = kubernetes_namespace.application_namespace.metadata[0].name
  project_id = var.project
  roles      = ["roles/storage.admin", "roles/compute.admin", "roles/monitoring.metricWriter", "roles/cloudtrace.agent", "roles/logging.logWriter"]
}

module "observability_workload_identity" {
  source     = "terraform-google-modules/kubernetes-engine/google//modules/workload-identity"
  name       = "${var.stack_name}-observability"
  namespace  = kubernetes_namespace.observability_namespace.metadata[0].name
  project_id = var.project
  roles      = ["roles/storage.admin", "roles/compute.admin", "roles/monitoring.metricWriter", "roles/cloudtrace.agent", "roles/logging.logWriter"]
}