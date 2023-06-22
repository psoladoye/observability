#!/usr/bin/env bash

#set -e

export OBSERVABILITY_RELEASE=observability-stack

export IMAGE_VERSION=1.0.4.4
docker build --push --platform linux/amd64 -t gcr.io/"${GCP_PROJECT}"/observability:"${IMAGE_VERSION}" .