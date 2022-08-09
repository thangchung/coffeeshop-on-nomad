#!/usr/bin/env bash
set -euo pipefail

require() {
  if ! hash "$1" &>/dev/null; then
    echo "'$1' not found in PATH"
    exit 1
  fi
}

require consul
require nomad

cleanup() {
  echo
  echo "Shutting down services"
  kill $(jobs -p)
  wait
}
trap cleanup EXIT

# https://github.com/deislabs/hippo/blob/de73ae52d606c0a2351f90069e96acea831281bc/src/Infrastructure/Jobs/NomadJob.cs#L28
# https://www.nomadproject.io/docs/drivers/exec#client-requirements
case "$OSTYPE" in
  linux*) SUDO="sudo --preserve-env=PATH" ;;
  *) SUDO= ;;
esac

# change to the directory of this script
cd "$(dirname "${BASH_SOURCE[0]}")"

${SUDO} rm -rf ./data
mkdir -p log

echo "Starting consul..."
${SUDO} consul agent -dev -client 0.0.0.0 -ui \
  -config-file ./etc/consul.hcl \
  -bootstrap-expect 1 \
  &>log/consul.log &

echo "Waiting for consul..."
while ! consul members &>/dev/null; do
  sleep 2
done

echo "Starting nomad..."
${SUDO} nomad agent -dev-connect \
  -config ./etc/nomad.hcl \
  -data-dir "${PWD}/data/nomad" \
  -consul-address "127.0.0.1:8500" \
  &>log/nomad.log &

echo "Waiting for nomad..."
while ! nomad server members 2>/dev/null | grep -q alive; do
  sleep 2
done

echo
echo "Done!!!"

wait