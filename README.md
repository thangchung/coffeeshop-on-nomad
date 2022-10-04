# coffeeshop-on-nomad

The .NET coffeeshop application runs on Nomad and Consul Connect

# Services

<table>
<thead>
    <td>No.</td>
    <td>Service Name</td>
    <td>URI</td>
</thead>
<tr>
    <td>1</td>
    <td>product-service</td>
    <td>http://localhost:5001 and http://localhost:15001</td>
</tr>
<tr>
    <td>2</td>
    <td>counter-service</td>
    <td>http://localhost:5002</td>
</tr>
<tr>
    <td>3</td>
    <td>barista-service</td>
    <td>http://localhost:5003</td>
</tr>
<tr>
    <td>4</td>
    <td>kitchen-service</td>
    <td>http://localhost:5004</td>
</tr>
<tr>
    <td>5</td>
    <td>reverse-proxy</td>
    <td>http://localhost:5000</td>
</tr>
<tr>
    <td>6</td>
    <td>signalr-web</td>
    <td>http://localhost:3000</td>
</tr>
<tr>
    <td>7</td>
    <td>datagen-app</td>
    <td></td>
</tr>
</table>

# Get starting

Install [Virtual Box](https://www.virtualbox.org/wiki/Downloads) and [Vagrant](https://www.vagrantup.com/docs/installation) on your host. Then run commands below:

```bash
> cd nomad
> vagrant up
```

Then, waiting until all jobs are available. All the endpoints below:
- Nomad Dashboard: [http://localhost:4646](http://localhost:4646)
- Consul UI: [http://localhost:8500](http://localhost:8500)
- Traefik Dashboard: [http://localhost:8888](http://localhost:8888)
- RabbitMQ UI: [http://localhost:15672](http://localhost:15672)

Using [client.http](client.http) to explore the application!

# devcontainer setup

F1 in vscode and choose `Remote-Containers: Open Folder in Container...`, then waiting until it build sucessful

```bash
# check linux version
> cat /etc/os-release
# build all images of application, should run with `root` user
$ docker compose build 
```

Open a new tab

```bash
> cd local/
$ sudo chmod +x core-libs.sh start.sh
$ sudo ./core-libs.sh
# nomad cannot run on wsl2 image, then we need to work-around
$ sudo mkdir -p /lib/modules/$(uname -r)/
> echo '_/bridge.ko' | sudo tee -a /lib/modules/$(uname -r)/modules.builtin
# Start nomad and consul
> ./start.sh
```

Open another new tab

```bash
> cd nomad/jobs
> nomad job run traefik.nomad.hcl
> nomad job run postgresdb.nomad.hcl
> nomad job run rabbitmq.nomad.hcl
> nomad job run product-api.nomad.hcl
> nomad job run counter-api.nomad.hcl
> nomad job run barista-api.nomad.hcl
> nomad job run kitchen-api.nomad.hcl
```

Finally, you can play around using [client.http](client.http) to explore the application!

# Troubleshooting

## Couldn't run `sebp/elk:latest` on Docker (Windows 11 - WSL2 with Docker for Desktop integrated)

> error: elasticsearch_1  | max virtual memory areas vm.max_map_count [65530] is too low, increase to at least [262144]

Jump into wsl2, then run command below

```
$ sudo sysctl -w vm.max_map_count=262144
```

Now, we can run `docker-compose up` again.

## `devcontainer`: `socat` IP address of docker to host IP

With `docker-from-docker` feature, we couldn't access the localhost of `docker` on host IP so that we have to cast the IP of docker inside to Host IP using `socat`

1. Get host IP using

```bash
> ifconfig
# make sure you get eth0 IP address, e.g. 192.168.255.214
```

2. Inside `devcontainer` with `docker-from-docker` feature

```bash
# make sure using root user inside devcontainer
$ socat TCP4-LISTEN:8080 TCP4:192.168.255.214:8080
```
