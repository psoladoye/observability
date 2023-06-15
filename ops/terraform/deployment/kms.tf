#module "k8s_kms" {
#  source              = "terraform-google-modules/kms/google"
#  project_id          = var.project
#  keyring             = "${var.stack_name}-keyring"
#  location            = var.region
#  keys                = [local.k8s_database_key]
#  key_rotation_period = "2592000s"
#  # keys can be destroyed by Terraform
#  prevent_destroy = false
#}