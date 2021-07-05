terraform {
  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "3.48"
    }
  }
}

provider "aws" {
  profile = "default"
  region  = "us-east-2"
}

variable "docker_tag" {
  type = string
}

# VPC
# module "cribbly_vpc" {
#   source = "terraform-aws-modules/vpc/aws"
#   name   = "cribbly-vpc"

#   azs              = ["us-east-2a", "us-east-2b", "us-east-2c"]
#   cidr             = "10.99.0.0/18"
#   public_subnets   = ["10.99.0.0/24", "10.99.1.0/24", "10.99.2.0/24"]
#   private_subnets  = ["10.99.3.0/24", "10.99.4.0/24", "10.99.5.0/24"]
#   database_subnets = ["10.99.7.0/24", "10.99.8.0/24", "10.99.9.0/24"]

#   create_database_subnet_group = true

#   enable_dns_support   = true
#   enable_dns_hostnames = true
# }

module "cribbly_lambda_function" {
  source = "terraform-aws-modules/lambda/aws"

  function_name = "cribbly_backend_lambda"
  description   = "Cribbly Backend Lambda"

  create_package = false

  image_uri    = "977779885689.dkr.ecr.us-east-2.amazonaws.com/cribbly-backend:${var.docker_tag}"
  package_type = "Image"
}
