#!/bin/bash

set -ex

source ./scripts/variables.sh

function run_tests() {
	dotnet test CribblyBackend.UnitTests
}

function build_project() {
    dotnet publish src/CribblyBackend -c Release -o $BUILD_OUTPUT_DIR
}

function publish_container {
    aws ecr get-login-password --region $AWS_REGION | docker login --username AWS --password-stdin $ECR_URL
    docker build -t $ECR_REGISTRY:$BUILD_NAME .
    docker push $ECR_REGISTRY:$BUILD_NAME
}

function upload_artifact() {
    if [ $IS_PR = false ]; then
        pushd $BUILD_OUTPUT_DIR
            zip -r $ARTIFACT_PATH *
        popd
        aws s3 cp $ARTIFACT_PATH $S3_ARTIFACT_URI
    else
        echo "PR build; skipping artifact upload..."
    fi
}

function main() {
    run_tests
    build_project
    # publish_container
}

main