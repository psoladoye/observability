#!/usr/bin/env bash

set -e

cd ops/terraform

export CI_ENVIRONMENT_SLUG=dev
export TF_BUCKET=tf-state-dev-007

export WORKSPACE="${CI_ENVIRONMENT_SLUG}"

terraform init -backend-config="bucket=${TF_BUCKET}"
terraform workspace select -or-create ${WORKSPACE}

cd ../..
