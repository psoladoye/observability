data "google_client_config" "default" {}

provider "kubernetes" {
  host                   = "https://${module.gke.endpoint}"
  token                  = data.google_client_config.default.access_token
  cluster_ca_certificate = base64decode(module.gke.ca_certificate)
}

resource "random_string" "suffix" {
  length  = 3
  special = false
  upper   = false
}

module "gke" {
  source             = "terraform-google-modules/kubernetes-engine/google"
  project_id         = var.project
  name               = local.gke_cluster_name
  regional           = false
  region             = var.region
  zones              = [var.zone]
  release_channel    = "STABLE"
  kubernetes_version = "1.25.8-gke.1000"

  network           = module.gcp_network.network_name
  subnetwork        = local.subnet_names[index(module.gcp_network.subnets_names, local.subnet_name)]
  ip_range_pods     = local.ip_range_pods
  ip_range_services = local.ip_range_services

  remove_default_node_pool = true
  initial_node_count       = 1

  master_authorized_networks = var.master_authorized_networks

  node_metadata         = "GKE_METADATA"
  grant_registry_access = true

  node_pools_oauth_scopes = {
    (local.main_node_pool) = [
      "https://www.googleapis.com/auth/logging.write",
      "https://www.googleapis.com/auth/monitoring",
      "https://www.googleapis.com/auth/trace.append",
      "https://www.googleapis.com/auth/devstorage.read_write"
    ]

    (local.sandbox_node_pool) = [
      "https://www.googleapis.com/auth/logging.write",
      "https://www.googleapis.com/auth/monitoring",
      "https://www.googleapis.com/auth/trace.append",
    ]
  }

  node_pools = [
    {
      name         = local.main_node_pool
      machine_type = local.machine_type
      max_count    = 3
      min_count    = 1
      image_type   = "COS_CONTAINERD"
      preemtible   = true
      disk_size_gb = 50
    },
    {
      name         = local.sandbox_node_pool
      machine_type = local.machine_type
      max_count    = 3
      min_count    = 1
      image_type   = "COS_CONTAINERD"
      preemtible   = true
      disk_size_gb = 50
    }
  ]
}