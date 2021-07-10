terraform {
  required_providers {
    digitalocean = {
      source  = "digitalocean/digitalocean"
      version = "~> 2.0"
    }
  }

  backend "s3" {
    bucket = "cribbly-backend-terraform-state"
    key    = "cribbly.tfstate"
    region = "us-east-2"
  }
}

resource "digitalocean_ssh_key" "terraform" {
  name       = "terraform"
  public_key = var.ssh_pub_key
}

# Set the variable value in *.tfvars file
# or using -var="do_token=..." CLI option
variable "do_token" {
  type = string
}

variable "db_password" {
  type = string
}

variable "ssh_key" {
  type = string
}

variable "ssh_pub_key" {
  type = string
}

# Configure the DigitalOcean Provider
provider "digitalocean" {
  token = var.do_token
}

# Create a new Web Droplet in the nyc2 region
resource "digitalocean_droplet" "web" {
  image  = "ubuntu-20-04-x64"
  name   = "web-1"
  region = "nyc1"
  size   = "s-1vcpu-1gb"
  ssh_keys = [
    digitalocean_ssh_key.terraform.fingerprint
  ]

  # still need to set up SSH keys to connect from GitHub actions machine to DO droplet
  connection {
    host        = self.ipv4_address
    user        = "root"
    type        = "ssh"
    private_key = var.ssh_key
    timeout     = "2m"
  }

  provisioner "remote-exec" {
    inline = [
      "apt install make",
      "snap install docker",
      "git clone https://github.com/cszczepaniak/cribbly-backend.git",
      "cd cribbly-backend",
      "git checkout dockerize",
      "echo 'DB_PASSWORD=${var.db_password}' >> ./config/dev.env",
      "make start",
    ]
  }
}
