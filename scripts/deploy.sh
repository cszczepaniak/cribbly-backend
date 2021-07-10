#!/bin/bash

set -ex

TF_VAR_do_token=$DO_TOKEN
TF_VAR_db_password=$DB_PASSWORD

cd infrastructure/
terraform init -input=false
terraform plan -out=tfplan -input=false
terraform apply -input=false tfplan