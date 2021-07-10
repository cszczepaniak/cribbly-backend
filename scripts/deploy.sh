#!/bin/bash

set -ex

export TF_VAR_do_token=$DO_TOKEN
export TF_VAR_db_password=$DB_PASSWORD
export TF_VAR_ssh_key=$SSH_KEY
export TF_VAR_ssh_pub_key=$SSH_PUB_KEY

cd infrastructure/
terraform init -input=false
terraform plan -out=tfplan -input=false
terraform apply -input=false tfplan