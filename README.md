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
        <td>reverse-proxy (local development only)</td>
        <td>http://localhost:5000</td>
    </tr>
    <tr>
        <td>6</td>
        <td>signalr-web (local development only)</td>
        <td>http://localhost:3000</td>
    </tr>
    <tr>
        <td>7</td>
        <td>datagen-app (local development only)</td>
        <td></td>
    </tr>
</table>

# Get starting

Control plane UI:

- Nomad Dashboard: [http://localhost:4646](http://localhost:4646)
- Consul UI: [http://localhost:8500](http://localhost:8500)
- Traefik Dashboard: [http://localhost:8080](http://localhost:8080)
- RabbitMQ UI: [http://localhost:15672](http://localhost:15672)

Using [client.http](client.http) to explore the application!

# devcontainer setup

F1 in vscode and choose `Remote-Containers: Open Folder in Container...`, then waiting until it build sucessful

```bash
# check linux version
> cat /etc/os-release
# build all images of application
> docker compose build 
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

## Clean up

```bash
> nomad job stop kitchen-api.nomad.hcl
> nomad job stop barista-api.nomad.hcl
> nomad job stop counter-api.nomad.hcl
> nomad job stop product-api.nomad.hcl
> nomad job stop rabbitmq.nomad.hcl
> nomad job stop postgresdb.nomad.hcl
> nomad job stop traefik.nomad.hcl
```

# Troubleshooting

## Couldn't run `sebp/elk:latest` on Docker (Windows 11 - WSL2 with Docker for Desktop integrated)

> error: elasticsearch_1  | max virtual memory areas vm.max_map_count [65530] is too low, increase to at least [262144]

Jump into wsl2, then run command below

```
$ sudo sysctl -w vm.max_map_count=262144
```

Now, we can run `docker-compose up` again.

# References

- Traefix dashboad: https://traefik.io/blog/traefik-proxy-fully-integrates-with-hashicorp-nomad/
- Rewrite URL on Nomad: https://doc.traefik.io/traefik/migration/v1-to-v2/#strip-and-rewrite-path-prefixes
