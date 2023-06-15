module "gcp_network" {
  source  = "terraform-google-modules/network/google"
  version = ">= 4.0.1"

  project_id   = var.project
  network_name = local.network_name

  subnets = [
    {
      subnet_name   = "cluster-subnet"
      subnet_ip     = "10.0.0.0/17"
      subnet_region = var.region
    },
    {
      subnet_name   = "cluster-master-subnet"
      subnet_ip     = "10.60.0.0/17"
      subnet_region = var.region
    },
  ]

  secondary_ranges = {
    "cluster-subnet" = [
      {
        range_name    = local.ip_range_pods
        ip_cidr_range = "192.168.0.0/18"
      },
      {
        range_name    = local.ip_range_services
        ip_cidr_range = "192.168.64.0/18"
      },
    ]
  }
}