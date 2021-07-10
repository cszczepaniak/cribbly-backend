export ROOT_DIR=$(pwd)
export BUILD_OUTPUT_DIR="${ROOT_DIR}/site"
export BUILD_NAME=${GITHUB_SHA:0:7}
export ARTIFACT_PATH="${ROOT_DIR}/$BUILD_NAME.zip"
export S3_ARTIFACT_URI="s3://cribbly-backend-artifacts/$BUILD_NAME.zip"
export ECR_URL="${AWS_ACCOUNT}.dkr.ecr.${AWS_REGION}.amazonaws.com"
export ECR_REGISTRY="${ECR_URL}/cribbly-backend"
export DB_NAME="cribblydb"

if [ "$GITHUB_EVENT_NAME" = "pull_request" ]; then
    export IS_PR=true
else
    export IS_PR=false
fi