job "server-api" {
  datacenters = ["dc1"]

  constraint {
    attribute = "${attr.kernel.name}"
    value     = "linux"
  }

  group "svc" {
    count = 2
    
    network {
      mode = "bridge"

      port "http" { }

      port "grpc" { }
    }

    service {
      name = "server-api-http"
      port = "http"
      tags = ["http"]

      connect {
        sidecar_service {}
      }
    }

    service {
      name = "server-api"
      port = "grpc"
      tags = ["grpc"]

      connect {
        sidecar_service {}
      }
    }

    task "server-api" {
      driver = "docker"

      config {
        image = "ghcr.io/thangchung/coffeeshop-on-nomad/grpc-dns-server:0.0.1"
        // force_pull = true
        ports = [ "http", "grpc" ]
      }

      env {
        ASPNETCORE_ENVIRONMENT = "Development"
        COREHOST_TRACE = 1
        RestPort = "${NOMAD_PORT_http}"
        GrpcPort = "${NOMAD_PORT_grpc}"
      }

      resources {
        cpu    = 100
        memory = 200
      }
    }
  }
}
