#!/usr/bin/env bash

set -e

cd ops/terraform

#export TF_LOG=debug

terraform plan
terraform apply -auto-approve

CLUSTER_NAME=$(terraform output -raw cluster_name)
NODE_SERVICE_ACCOUNT=$(terraform output -raw node_service_account)
K8S_SERVICE_ACCOUNT=$(terraform output -raw k8s_service_account)
K8S_SERVICE_ACCOUNT_NAME=$(terraform output -raw k8s_service_account_name)
APPLICATION_NAMESPACE=$(terraform output -raw application_namespace)
OBSERVABILITY_NAMESPACE=$(terraform output -raw observability_namespace)
GCP_PROJECT=$(terraform output -raw gcp_project)
GCP_ZONE=$(terraform output -raw gcp_zone)

export CLUSTER_NAME
export NODE_SERVICE_ACCOUNT
export K8S_SERVICE_ACCOUNT
export K8S_SERVICE_ACCOUNT_NAME
export APPLICATION_NAMESPACE
export OBSERVABILITY_NAMESPACE
export GCP_PROJECT
export GCP_REGION="us-central1"
export GCP_ZONE

cd ../..

# store url for gitlab environment
#echo "PUBLIC_URL=$PUBLIC_URL" >> .env