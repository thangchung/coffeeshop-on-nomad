job "product-api" {
  datacenters = ["dc1"]

  group "product-api" {
    count = 2
    
    network {
      mode = "bridge"

      // port "product_api_http" { }

      // port "product_api_grpc" { }
    }

    // service {
    //   name = "product-api"
    //   port = "5001"

    //   connect {
    //     sidecar_service {}
    //   }

    //   tags = [
    //     "traefik.enable=true",
    //     "traefik.consulcatalog.connect=true",
    //     "traefik.http.routers.productapi.rule=PathPrefix(`/product-api`)",
    //     "traefik.http.routers.productapi.middlewares=productapi-stripprefix",
    //     "traefik.http.middlewares.productapi-stripprefix.stripprefix.prefixes=/product-api",
    //   ]
    // }

    service {
      name = "product-api"
      port = "15001"

      connect {
        sidecar_service {}
      }
    }

    task "product-api" {
      driver = "docker"

      config {
        image = "ghcr.io/thangchung/coffeeshop-on-nomad/product-service:0.1.0"
        // force_pull = true
      }

      env {
        ASPNETCORE_ENVIRONMENT = "Development"
        RestPort="5001"
        GrpcPort="15001"
        UseTracingExporter = "console1"
        UseMetricsExporter = "console1"
        UseLogExporter = "console1"
        AspNetCoreInstrumentation__RecordException = true
      }

      resources {
        cpu    = 100
        memory = 128
      }
    }
  }
}
