#!/usr/bin/env bash

#set -e

export OBSERVABILITY_RELEASE=observability-stack

export IMAGE_VERSION=1.0.3
#docker build -t gcr.io/"${GCP_PROJECT}"/observability:"${IMAGE_VERSION}" .
#docker push gcr.io/"${GCP_PROJECT}"/observability:"${IMAGE_VERSION}"