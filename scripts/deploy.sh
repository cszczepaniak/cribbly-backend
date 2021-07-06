#!/bin/bash

set -ex

source ./scripts/variables.sh

export TF_VAR_image_uri=$CONTAINER_URI
export TF_VAR_aws_region=$AWS_REGION
export TF_VAR_db_name=$DB_NAME