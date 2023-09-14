resource "google_service_account" "foodremedy_clusterServiceAccount" {
  account_id   = "service-account-id"
  display_name = "Service Account"
}

resource "google_container_cluster" "foodremedy_cluster" {
  name                     = "${var.env}-foodremedy-cluster"
  location                 = var.region
  initial_node_count       = 1
  remove_default_node_pool = true
}

resource "google_container_node_pool" "foodremedy_clusterNodePool" {
  name       = "${var.env}-foodremedy-cluster-node-pool"
  location   = var.region
  cluster    = google_container_cluster.foodremedy_cluster.name
  node_count = 1

  node_config {
    preemptible  = true
    machine_type = "e2-medium"

    service_account = google_service_account.foodremedy_clusterServiceAccount.email
    oauth_scopes = [
      "https://www.googleapis.com/auth/cloud-platform"
    ]
  }
}
