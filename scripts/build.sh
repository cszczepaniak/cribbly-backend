#!/bin/bash

set -ex

source ./scripts/variables.sh

function run_tests() {
	dotnet test CribblyBackend.UnitTests
}

function docker_login() {
    aws ecr get-login-password --region "$AWS_REGION"| docker login --username AWS --password-stdin "$ECR_REPO_URI"
}

function build_project() {
    docker build -t "cribbly-backend:$BUILD_NAME"
}

function upload_docker_image() {
    if [ $IS_PR = true ]; then # TODO TURN THIS BACK TO FALSE!!!!!
        docker tag "cribbly-backend:$BUILD_NAME" "$CONTAINER_URI"
        docker push "$CONTAINER_URI"
    else
        echo "PR build; skipping container upload..."
    fi
}

function main() {
    run_tests
    docker_login
    build_project
    upload_docker_image
}

main