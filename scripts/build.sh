#!/bin/bash

set -ex

source ./scripts/variables.sh

function run_tests() {
	dotnet test CribblyBackend.UnitTests
}

function build_project() {
    dotnet publish src/CribblyBackend -c Release
}

function upload_artifact() {
    zip $BUILD_NAME.zip -r src/CribblyBackend/bin/Release/net5.0/publish
    aws s3 cp $BUILD_NAME.zip $ARTIFACT_BUCKET/$BUILD_NAME.zip
}

function main() {
    run_tests
    build_project
    upload_artifact
}