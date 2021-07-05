#!/bin/bash

set -ex

source ./scripts/variables.sh

function run_tests() {
	dotnet test CribblyBackend.UnitTests
}

function build_project() {
    dotnet release src/CribblyBackend
    docker build -t "$CONTAINER_URI" src/CribblyBackend
}

function upload_docker_image() {
    if [ $IS_PR = true ]; then # TODO TURN THIS BACK TO FALSE!!!!!
        docker push "$CONTAINER_URI"
    else
        echo "PR build; skipping container upload..."
    fi
}

function main() {
    run_tests
    build_project
    upload_docker_image
}

main