#!/bin/bash

set -ex

function run_tests() {
	dotnet test CribblyBackend.UnitTests
}

function build_project() {
    dotnet publish src/CribblyBackend -c Release
}
function main() {
    run_tests
    build_project
}

main