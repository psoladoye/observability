module "gcp_network" {
  source  = "terraform-google-modules/network/google"
  version = ">= 4.0.1"

  project_id   = var.project
  network_name = local.network_name

  subnets = [
    {
      subnet_name   = local.subnet_name
      subnet_ip     = "10.3.0.0/17"
      subnet_region = var.region
    }
  ]

  secondary_ranges = {
    (local.subnet_name) = [
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