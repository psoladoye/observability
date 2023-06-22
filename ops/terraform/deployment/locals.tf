locals {
  network_name            = var.stack_name
  subnet_name             = "${var.stack_name}-subnet"
  master_auth_subnet_name = "${var.stack_name}-master-subnet"
  subnet_names            = [for subnet_self_link in module.gcp_network.subnets_self_links : split("/", subnet_self_link)[length(split("/", subnet_self_link)) - 1]]
  ip_range_pods           = "ip-range-pods-${random_string.suffix.result}"
  ip_range_services       = "ip-range-svc-${random_string.suffix.result}"

  machine_type = "e2-standard-2"

  application_namespace   = "app"
  observability_namespace = "observability"

  gke_cluster_name = "${var.stack_name}-cluster"
  k8s_database_key = "${var.stack_name}-k8s-database-key"

  main_node_pool    = "${var.stack_name}-main-pool"
  sandbox_node_pool = "${var.stack_name}-sb-pool"
}