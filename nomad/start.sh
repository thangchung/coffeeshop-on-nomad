#!/usr/bin/env bash
set -euo pipefail

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
sudo usermod -aG docker vagrant

# install cni plugins https://www.nomadproject.io/docs/integrations/consul-connect#cni-plugins
echo "Installing cni plugins..."
curl -L -o cni-plugins.tgz "https://github.com/containernetworking/plugins/releases/download/v1.1.1/cni-plugins-linux-$( [ $(uname -m) = aarch64 ] && echo arm64 || echo amd64)"-v1.1.1.tgz
sudo mkdir -p /opt/cni/bin
sudo tar -C /opt/cni/bin -xzf cni-plugins.tgz

echo "Installing Consul..."
apt-get install consul=1.12.2-1 -y

echo "Starting Consul dev server..."
consul agent -dev -client 0.0.0.0 -ui > consul.log 2>&1 &

echo "Installing Nomad..."
apt-get install nomad=1.3.1-1 -y

echo "Starting Nomad dev server..."
VAULT_TOKEN=root nomad agent -dev-connect > nomad.log 2>&1 &

echo "Installing Vault..."
apt-get install vault=1.11.0-1 -y

# configuring environment
sudo -H -u vagrant nomad -autocomplete-install
sudo -H -u vagrant consul -autocomplete-install
sudo -H -u vagrant vault -autocomplete-install
sudo tee -a /etc/environment <<EOF
export VAULT_ADDR=http://localhost:8200
export VAULT_TOKEN=root
EOF

source /etc/environment

echo "Starting Vault dev server..."
vault server -dev -dev-listen-address="0.0.0.0:8200" -dev-root-token-id="root" > vault.log 2>&1 &
sleep 5
vault status

# enable vault audit logs
touch /var/log/vault_audit.log
vault audit enable file file_path=/var/log/vault_audit.log

echo "Running Nomad jobs..."
nomad run jobs/traefik.nomad.hcl
nomad run jobs/postgresdb.nomad.hcl
nomad run jobs/rabbitmq.nomad.hcl
# nomad run jobs/fabio.nomad.hcl
nomad run jobs/product-api.nomad.hcl
nomad run jobs/counter-api.nomad.hcl
nomad run jobs/barista-api.nomad.hcl
nomad run jobs/kitchen-api.nomad.hcl
nomad run jobs/datagen.nomad.hcl