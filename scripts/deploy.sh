#!/bin/bash

set -ex

export TF_VAR_do_token=$DO_TOKEN
export TF_VAR_db_password=$DB_PASSWORD

cd infrastructure/
terraform init -input=false
terraform plan -out=tfplan -input=false
terraform apply -input=false tfplan