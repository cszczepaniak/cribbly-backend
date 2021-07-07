#!/bin/bash

set -ex

source ./scripts/variables.sh

COMMIT_SHA=${1:0:7}
export TF_VAR_aws_region=$AWS_REGION
export TF_VAR_build_name=$BUILD_NAME
export TF_VAR_artifact_path=$ARTIFACT_PATH


if [ $IS_PR = true ]; then
    cd ./infrastructure/
    terraform init -input=false
    terraform plan -out=tfplan -input=false
    terraform apply -input=false tfplan
else
    echo "PR build; skipping deploy..."
fi