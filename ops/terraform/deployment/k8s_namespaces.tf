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

resource "kubernetes_namespace" "observability_namespace" {
  depends_on = [null_resource.get-credentials]

  metadata {
    name = local.observability_namespace
  }
}