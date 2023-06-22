#!/usr/bin/env bash

#set -e

cd ops/terraform || exit

#export TF_LOG=debug

terraform plan
terraform apply -auto-approve

CLUSTER_NAME=$(terraform output -raw cluster_name)
NODE_SERVICE_ACCOUNT=$(terraform output -raw node_service_account)
K8S_APP_SERVICE_ACCOUNT=$(terraform output -raw k8s_app_service_account)
K8S_APP_SERVICE_ACCOUNT_NAME=$(terraform output -raw k8s_app_service_account_name)
K8S_OBSERVABILITY_SERVICE_ACCOUNT=$(terraform output -raw k8s_observability_service_account)
K8S_OBSERVABILITY_SERVICE_ACCOUNT_NAME=$(terraform output -raw k8s_observability_service_account_name)
APPLICATION_NAMESPACE=$(terraform output -raw application_namespace)
OBSERVABILITY_NAMESPACE=$(terraform output -raw observability_namespace)
GCP_PROJECT=$(terraform output -raw gcp_project)
GCP_ZONE=$(terraform output -raw gcp_zone)
MAIN_NODE_POOL=$(terraform output -raw main_node_pool)
SANDBOX_NODE_POOL=$(terraform output -raw sandbox_node_pool)

export CLUSTER_NAME
export NODE_SERVICE_ACCOUNT
export K8S_APP_SERVICE_ACCOUNT
export K8S_APP_SERVICE_ACCOUNT_NAME
export K8S_OBSERVABILITY_SERVICE_ACCOUNT
export K8S_OBSERVABILITY_SERVICE_ACCOUNT_NAME
export APPLICATION_NAMESPACE
export OBSERVABILITY_NAMESPACE
export GCP_PROJECT
export GCP_REGION="us-central1"
export GCP_ZONE
export MAIN_NODE_POOL
export SANDBOX_NODE_POOL

cd ../..

# store url for gitlab environment
#echo "PUBLIC_URL=$PUBLIC_URL" >> .env