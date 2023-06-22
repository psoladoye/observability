#!/usr/bin/env bash

#set -e

# Connect to Kubernetes Via GKE
# ======================================================================================================================
#gcloud auth activate-service-account "${DEPLOY_GSA}" --key-file="${GOOGLE_CREDENTIALS}"
gcloud container clusters get-credentials "${CLUSTER_NAME}" --zone "${GCP_ZONE}" --project "${GCP_PROJECT}"

# Upgrade Observability Release
# ======================================================================================================================
envsubst < ops/helm/observability-stack/x-values-tmp.yaml > ops/helm/observability-stack/values.yaml
helm dependency update ops/helm/observability-stack
helm upgrade "${OBSERVABILITY_RELEASE}" ops/helm/observability-stack --install --namespace="${OBSERVABILITY_NAMESPACE}"

# Upgrade Application Release
# ======================================================================================================================    
envsubst < ops/helm/sample-app/x-values-tmp.yaml > ops/helm/sample-app/values.yaml
helm upgrade sample-app ops/helm/sample-app --install --namespace="${APPLICATION_NAMESPACE}"