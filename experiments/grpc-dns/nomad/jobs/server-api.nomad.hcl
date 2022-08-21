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

      // port "http" {
      //   to = 5000
      // }

      port "grpc" {
        to = 15000
      }
    }

    // service {
    //   name = "server-api"
    //   port = "5000"

    //   connect {
    //     sidecar_service {}
    //   }
    // }

    service {
      name = "server-api"
      port = "15000"

      // connect {
      //   sidecar_service {}
      // }
    }

    task "server-api" {
      driver = "docker"

      config {
        image = "ghcr.io/thangchung/coffeeshop-on-nomad/grpc-dns-server:0.0.1"
        // force_pull = true
      }

      env {
        ASPNETCORE_ENVIRONMENT = "Development"
        COREHOST_TRACE = 1
        RestPort = 5000
        GrpcPort = 15000
      }

      resources {
        cpu    = 100
        memory = 200
      }
    }
  }
}
