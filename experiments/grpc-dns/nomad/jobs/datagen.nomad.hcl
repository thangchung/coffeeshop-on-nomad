job "datagen" {
  datacenters = ["dc1"]

  constraint {
    attribute = "${attr.kernel.name}"
    value     = "linux"
  }

  group "datagen" {
    count = 1
    
    network {
      mode = "bridge"

      port "http" { }
    }

    service {
      name = "datagen"

      connect {
        sidecar_service {
          proxy {
            upstreams {
              destination_name = "server-api"
              local_bind_port  = 15000
            }
          }
        }
      }
    }

    task "datagen" {
      driver = "docker"

      config {
        image = "ghcr.io/thangchung/coffeeshop-on-nomad/grpc-dns-client:0.0.1"
        // force_pull = true
      }

      env {
        ASPNETCORE_ENVIRONMENT = "Development"
        COREHOST_TRACE = 1
        UseGrpcDns = false
        ServerUri = "http://${NOMAD_UPSTREAM_ADDR_server_api}"
        // ConsulServerUri = "dns:///127.0.0.1:8600/server-api.service.consul"
        //ConsulServerUri = "dns:///127.0.0.1:8500/server-api.service.consul"
        //consul://user:passsword@127.0.0.1:8500/service_name
      }

      resources {
        cpu    = 100
        memory = 200
      }
    }
  }
}
