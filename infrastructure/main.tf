terraform {
  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "3.48"
    }
  }

  backend "s3" {
    bucket = "cribbly-backend-terraform-state"
    key    = "cribbly.tfstate"
    region = "us-east-2"
  }
}

variable "aws_region" {
  type = string
}

provider "aws" {
  profile = "default"
  region  = var.aws_region
}

variable "image_uri" {
  type = string
}


# VPC
module "cribbly_vpc" {
  source  = "terraform-aws-modules/vpc/aws"
  version = "~> 3.0"

  name = "cribbly-vpc"
  cidr = "10.99.0.0/18"

  azs              = ["${var.aws_region}a", "${var.aws_region}b", "${var.aws_region}c"]
  public_subnets   = ["10.99.0.0/24", "10.99.1.0/24", "10.99.2.0/24"]
  private_subnets  = ["10.99.3.0/24", "10.99.4.0/24", "10.99.5.0/24"]
  database_subnets = ["10.99.7.0/24", "10.99.8.0/24", "10.99.9.0/24"]
}

# RDS
variable "db_name" {
  type = string
}
resource "random_password" "rds_password" {
  length  = 16
  special = false
}

module "cribbly_aurora_mysql" {
  source = "terraform-aws-modules/rds-aurora/aws"

  name              = "cribbly-mysql"
  engine            = "aurora-mysql"
  engine_mode       = "serverless"
  storage_encrypted = true

  username      = "cribbly_user"
  password      = random_password.rds_password.result
  database_name = var.db_name

  vpc_id                = module.cribbly_vpc.vpc_id
  subnets               = module.cribbly_vpc.database_subnets
  create_security_group = true
  allowed_cidr_blocks   = module.cribbly_vpc.private_subnets_cidr_blocks

  replica_scale_enabled = false
  replica_count         = 0

  monitoring_interval = 60

  apply_immediately   = true
  skip_final_snapshot = false

  db_parameter_group_name         = aws_db_parameter_group.cribbly_mysql_params.id
  db_cluster_parameter_group_name = aws_rds_cluster_parameter_group.cribbly_rds_cluster_params.id

  scaling_configuration = {
    auto_pause               = true
    min_capacity             = 1
    max_capacity             = 4
    seconds_until_auto_pause = 300
    timeout_action           = "ForceApplyCapacityChange"
  }
}

resource "aws_db_parameter_group" "cribbly_mysql_params" {
  name        = "cribbly-aurora-db-mysql-parameter-group"
  family      = "aurora-mysql5.7"
  description = "cribbly-aurora-db-mysql-parameter-group"
}

resource "aws_rds_cluster_parameter_group" "cribbly_rds_cluster_params" {
  name        = "cribbly-aurora-mysql-cluster-parameter-group"
  family      = "aurora-mysql5.7"
  description = "cribbly-aurora-mysql-cluster-parameter-group"
}

# Lambda
module "cribbly_lambda_function" {
  source = "terraform-aws-modules/lambda/aws"

  function_name = "cribbly_backend_lambda"
  description   = "Cribbly Backend Lambda"

  create_package = false

  image_uri    = var.image_uri
  package_type = "Image"
  environment_variables = {
    DNS_HOST     = module.cribbly_aurora_mysql.rds_cluster_endpoint,
    DNS_USER     = "cribbly_user",
    DNS_PASSWORD = random_password.rds_password.result,
    DNS_PORT     = 3306,
    DB_NAME      = var.db_name,
  }
}

resource "aws_api_gateway_rest_api" "cribbly_api_gateway" {
  name        = "CribblyAPI"
  description = "The REST API for Cribbly"
}

resource "aws_api_gateway_resource" "cribbly_api_proxy" {
  rest_api_id = aws_api_gateway_rest_api.cribbly_api_gateway.id
  parent_id   = aws_api_gateway_rest_api.cribbly_api_gateway.root_resource_id
  path_part   = "{proxy+}"
}

resource "aws_api_gateway_method" "cribbly_api_resource_method" {
  rest_api_id   = aws_api_gateway_rest_api.cribbly_api_gateway.id
  resource_id   = aws_api_gateway_resource.cribbly_api_proxy.id
  http_method   = "ANY"
  authorization = "NONE"
}

resource "aws_api_gateway_integration" "cribbly_lambda_integration" {
  rest_api_id = aws_api_gateway_rest_api.cribbly_api_gateway.id
  resource_id = aws_api_gateway_method.cribbly_api_resource_method.resource_id
  http_method = aws_api_gateway_method.cribbly_api_resource_method.http_method

  integration_http_method = "ANY"
  type                    = "AWS_PROXY"
  uri                     = module.cribbly_lambda_function.lambda_function_invoke_arn
}

resource "aws_api_gateway_deployment" "cribbly_apig_deployment" {
  depends_on = [
    aws_api_gateway_integration.cribbly_lambda_integration,
  ]

  rest_api_id = aws_api_gateway_rest_api.cribbly_api_gateway.id
  stage_name  = "dev"
}

output "base_url" {
  value = aws_api_gateway_deployment.cribbly_apig_deployment.invoke_url
}

resource "aws_lambda_permission" "cribbly_apig" {
  statement_id  = "AllowAPIGatewayInvoke"
  action        = "lambda:InvokeFunction"
  function_name = module.cribbly_lambda_function.lambda_function_name
  principal     = "apigateway.amazonaws.com"
  source_arn    = "${aws_api_gateway_rest_api.cribbly_api_gateway.execution_arn}/*/*"
}
