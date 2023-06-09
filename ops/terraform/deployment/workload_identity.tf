module "workload_identity" {
  source     = "terraform-google-modules/kubernetes-engine/google//modules/workload-identity"
  name       = var.stack_name
  namespace  = var.application_namespace
  project_id = var.project
  roles      = ["roles/storage.admin", "roles/compute.admin"]
}