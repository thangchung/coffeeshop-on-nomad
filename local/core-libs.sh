#!/usr/bin/env bash
set -euo pipefail

# install apt-add-repository
apt update 
apt install software-properties-common -y
apt update

echo "Adding HashiCorp GPG key and repo..."
curl -fsSL https://apt.releases.hashicorp.com/gpg | apt-key add -
apt-add-repository "deb [arch=amd64] https://apt.releases.hashicorp.com $(lsb_release -cs) main"
apt-get update

echo "Adding Docker GPG key and repo..."
apt-get install apt-transport-https ca-certificates curl jq gnupg lsb-release -y
curl -fsSL https://download.docker.com/linux/ubuntu/gpg | gpg --dearmor -o /usr/share/keyrings/docker-archive-keyring.gpg
echo \
"deb [arch=amd64 signed-by=/usr/share/keyrings/docker-archive-keyring.gpg] https://download.docker.com/linux/ubuntu \
$(lsb_release -cs) stable" | tee /etc/apt/sources.list.d/docker.list > /dev/null
apt-get update

echo "Installing Docker..."
apt-get install docker-ce -y

# restart docker to make sure we get the latest version of the daemon if there is an upgrade
sudo service docker restart

# make sure we can actually use docker as the vagrant user
# sudo usermod -aG docker vagrant

# install cni plugins https://www.nomadproject.io/docs/integrations/consul-connect#cni-plugins
echo "Installing cni plugins..."
curl -L -o cni-plugins.tgz "https://github.com/containernetworking/plugins/releases/download/v1.1.1/cni-plugins-linux-$( [ $(uname -m) = aarch64 ] && echo arm64 || echo amd64)"-v1.1.1.tgz
sudo mkdir -p /opt/cni/bin
sudo tar -C /opt/cni/bin -xzf cni-plugins.tgz

echo "Installing Consul..."
sudo apt-get install consul=1.12.2-1 -y

echo "Installing Nomad..."
sudo apt-get install nomad=1.3.1-1 -y

# echo "Installing Vault..."
# sudo apt-get install vault=1.11.0-1 -y

# # configuring environment
# sudo -H -u root nomad -autocomplete-install
# sudo -H -u root consul -autocomplete-install
# sudo -H -u root vault -autocomplete-install
# sudo tee -a /etc/environment <<EOF
# export VAULT_ADDR=http://localhost:8200
# export VAULT_TOKEN=root
# EOF

source /etc/environment