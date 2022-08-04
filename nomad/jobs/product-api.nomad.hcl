job "product-api" {
  datacenters = ["dc1"]

  group "api" {
    count = 2
    
    network {
      mode = "bridge"

      port "product_api_http" {}

      port "product_api_grpc" {}
    }

    service {
      name = "product-api"
      port = "product_api_http"

      connect {
        sidecar_service {}
      }

      tags = [
        "traefik.enable=true",
        "traefik.consulcatalog.connect=true",
        "traefik.http.routers.productapi.rule=PathPrefix(`/product-api`)",
        "traefik.http.routers.productapi.middlewares=productapi-stripprefix",
        "traefik.http.middlewares.productapi-stripprefix.stripprefix.prefixes=/product-api",
      ]
    }

    service {
      name = "product-api"
      port = "product_api_grpc"

      connect {
        sidecar_service {}
      }
    }

    task "api-task" {
      driver = "docker"

      config {
        image = "ghcr.io/thangchung/coffeeshop-on-nomad/product-service:0.1.0"
      }

      env {
        ASPNETCORE_ENVIRONMENT = "Development"
        RestPort="${NOMAD_PORT_product_api_http}"
        GrpcPort="${NOMAD_PORT_product_api_grpc}"
      }

      resources {
        cpu    = 100
        memory = 128
      }
    }
  }
}
