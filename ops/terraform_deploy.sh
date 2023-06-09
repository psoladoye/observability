#!/usr/bin/env bash

set -e

cd terraform

#export TF_LOG=debug

terraform plan -out=tfplan
terraform apply -auto-approve "tfplan"

CLUSTER_NAME=$(terraform output -raw cluster_name)
SERVICE_ACCOUNT=$(terraform output -raw deployment_service_account)
APPLICATION_NAMESPACE=$(terraform output -raw application_namespace)
OBSERVABILITY_NAMESPACE=$(terraform output -raw observability_namespace)

export CLUSTER_NAME
export SERVICE_ACCOUNT
export APPLICATION_NAMESPACE
export OBSERVABILITY_NAMESPACE

cd ..

# store url for gitlab environment
#echo "PUBLIC_URL=$PUBLIC_URL" >> .env

echo "Cluster name: ${CLUSTER_NAME}"