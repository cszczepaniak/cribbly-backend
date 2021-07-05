export BUILD_NAME=${GITHUB_SHA:0:7}
export ARTIFACT_BUCKET="s3://cribbly-backend-artifacts"

if [ "$GITHUB_EVENT_NAME" = "pull_request" ]; then
    export IS_PR=true
else
    export IS_PR=false
fi