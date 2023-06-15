locals {
  stack_name         = "${substr(var.environment, 0, 20)}-${random_id.stack.hex}"
  machine_type       = "e2-standard-2"
  identity_namespace = "${var.project}.svc.id.goog"
}
