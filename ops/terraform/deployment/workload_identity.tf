# Get the credentials 
resource "null_resource" "get-credentials" {

  depends_on = [module.gke]

  provisioner "local-exec" {
    command = "gcloud container clusters get-credentials ${module.gke.name} --zone ${var.zone} --project ${var.project}"
  }
}

resource "kubernetes_namespace" "application_namespace" {

  depends_on = [null_resource.get-credentials]

  metadata {
    name = local.application_namespace
  }
}

module "workload_identity" {
  source     = "terraform-google-modules/kubernetes-engine/google//modules/workload-identity"
  name       = var.stack_name
  namespace  = local.application_namespace
  project_id = var.project
  roles      = ["roles/storage.admin", "roles/compute.admin"]
  depends_on = [kubernetes_namespace.application_namespace]
}