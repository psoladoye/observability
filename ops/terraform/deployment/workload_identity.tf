module "app_workload_identity" {
  source     = "terraform-google-modules/kubernetes-engine/google//modules/workload-identity"
  name       = "${var.stack_name}-app"
  namespace  = kubernetes_namespace.application_namespace.metadata[0].name
  project_id = var.project
  roles      = ["roles/storage.admin", "roles/compute.admin"]
}

module "observability_workload_identity" {
  source     = "terraform-google-modules/kubernetes-engine/google//modules/workload-identity"
  name       = "${var.stack_name}-observability"
  namespace  = kubernetes_namespace.observability_namespace.metadata[0].name
  project_id = var.project
  roles      = ["roles/storage.admin", "roles/compute.admin"]
}