export BUILD_NAME=${GITHUB_SHA:0:7}
export ECR_REPO_URI="$ECR_REGISTRY/cribbly-backend"
export CONTAINER_URI="$ECR_REPO_URI:$BUILD_NAME"

if [ "$GITHUB_EVENT_NAME" = "pull_request" ]; then
    export IS_PR=true
else
    export IS_PR=false
fi