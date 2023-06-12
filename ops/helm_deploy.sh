#!/usr/bin/env bash

set -e

# Connect to Kubernetes Via GKE
# ======================================================================================================================
#gcloud auth activate-service-account "${DEPLOY_GSA}" --key-file="${GOOGLE_CREDENTIALS}"
gcloud container clusters get-credentials "${CLUSTER_NAME}" --zone "${GCP_ZONE}" --project "${GCP_PROJECT}"

envsubst < ops/helm/observability-app/values-tmp.yaml > ops/helm/observability-app/values.yaml

# Upgrade Application Release
# ======================================================================================================================    
helm upgrade observability-app ops/helm/observability-app --install --namespace="${APPLICATION_NAMESPACE}"