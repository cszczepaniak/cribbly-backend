#!/bin/bash

set -ex

source ./scripts/variables.sh

COMMIT_SHA=${1:0:7}
export TF_VAR_image_uri="$ECR_REPO_URI:$COMMIT_SHA"
export TF_VAR_aws_region=$AWS_REGION
export TF_VAR_db_name=$DB_NAME


if [ $IS_PR = false ]; then
    terraform init -input=false
    terraform plan -out=tfplan -input=false
    terraform apply -input=false tfplan
else
    echo "PR build; skipping deploy..."
fi