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

variable "build_name" {
  type = string
}

variable "artifact_path" {
  type = string
}

variable "db_user" {
  type    = string
  default = "cribbly_user"
}

provider "aws" {
  profile = "default"
  region  = var.aws_region
}

resource "aws_s3_bucket" "cribbly_backend_artifact_bucket" {
  bucket = "cribbly-backend-artifacts"
}

resource "aws_s3_bucket_object" "cribbly_backend_artifact" {
  bucket = aws_s3_bucket.cribbly_backend_artifact_bucket.id
  key    = "${var.build_name}.zip"
}

resource "aws_acm_certificate" "cribbly_ssl_cert" {
  domain_name       = "*.cribbly.net"
  validation_method = "DNS"
}

resource "aws_elastic_beanstalk_application" "cribbly_app" {
  name        = "cribbly-backend"
  description = "The backend for Cribbly"
}

resource "aws_elastic_beanstalk_application_version" "cribbly_app_version" {
  name        = "cribbly-app-version"
  application = "cribbly-backend"
  description = "application version created by terraform"
  bucket      = aws_s3_bucket.cribbly_backend_artifact_bucket.id
  key         = aws_s3_bucket_object.cribbly_backend_artifact.id
}

resource "random_password" "db_password" {
  length  = 16
  special = false
}

resource "aws_elastic_beanstalk_environment" "cribbly_app_env" {
  name                = "cribbly-dev"
  application         = aws_elastic_beanstalk_application.cribbly_app.name
  solution_stack_name = "64bit Amazon Linux 2 v2.2.1 running .NET Core"

  setting {
    namespace = "aws:ec2:instances"
    name      = "InstanceTypes"
    value     = "t2.micro"
  }

  setting {
    namespace = "aws:elasticbeanstalk:environment"
    name      = "LoadBalancerType"
    value     = "application"
  }

  setting {
    namespace = "aws:elbv2:listener:default"
    name      = "Protocol"
    value     = "HTTPS"
  }

  setting {
    namespace = "aws:elbv2:listener:default"
    name      = "Protocol"
    value     = "HTTPS"
  }

  setting {
    namespace = "aws:elbv2:listener:default"
    name      = "SSLCertificateArns"
    value     = aws_acm_certificate.cribbly_ssl_cert.id
  }

  setting {
    namespace = "aws:rds:instance"
    name      = "DBAllocatedStorage"
    value     = 20
  }

  setting {
    namespace = "aws:rds:instance"
    name      = "DBDeletionPolicy"
    value     = "Snapshot"
  }

  setting {
    namespace = "aws:rds:instance"
    name      = "DBEngine"
    value     = "mysql"
  }

  setting {
    namespace = "aws:rds:instance"
    name      = "DBInstanceClass"
    value     = "db.t2.micro"
  }

  setting {
    namespace = "aws:rds:instance"
    name      = "DBPassword"
    value     = random_password.db_password.result
  }

  setting {
    namespace = "aws:rds:instance"
    name      = "DBUser"
    value     = var.db_user
  }

  setting {
    namespace = "aws:rds:instance"
    name      = "MultiAZDatabase"
    value     = false
  }

  setting {
    namespace = "aws:elasticbeanstalk:application:environment"
    name      = "DNS_USER"
    value     = var.db_user
  }

  setting {
    namespace = "aws:elasticbeanstalk:application:environment"
    name      = "DNS_PASSWORD"
    value     = random_password.db_password.result
  }

  setting {
    namespace = "aws:elasticbeanstalk:application:environment"
    name      = "DNS_PORT"
    value     = 3306
  }

  setting {
    namespace = "aws:elasticbeanstalk:application:environment"
    name      = "DNS_HOST"
    value     = ""
  }

}

