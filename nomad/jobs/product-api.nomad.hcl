// https://discuss.hashicorp.com/t/several-consul-connect-upstreams/3739
// https://discuss.hashicorp.com/t/correct-way-to-connect-to-upstream-that-uses-dynamic-ports/23958/2

job "product-api" {
  datacenters = ["dc1"]

  group "svc" {
    count = 2
    
    network {
      mode = "bridge"

      port "http" {
        to = 5001
      }

      port "grpc" {
        to = 15001
      }
    }

    service {
      name = "product-api-http"
      port = "http"

      connect {
        sidecar_service {}
      }

      tags = [
        "traefik.enable=true",
        "traefik.consulcatalog.connect=true",
        "traefik.http.routers.http.rule=PathPrefix(`/product-api`)",
        "traefik.http.routers.http.middlewares=http-stripprefix",
        "traefik.http.middlewares.http-stripprefix.stripprefix.prefixes=/product-api",
      ]
    }

    service {
      name = "product-api-grpc"
      port = "grpc"

      tags = [
        "traefik.tcp.routers.grpc.rule=HostSNI(`*`)",
        "traefik.tcp.routers.grpc.entrypoints=grpc",
        "traefik.tcp.services.product-api-grpc.loadbalancer.server.port=15001",
        "traefik.enable=true",
      ]
    }

    task "product-api" {
      driver = "docker"

      config {
        image = "ghcr.io/thangchung/coffeeshop-on-nomad/product-service:0.1.1"
        // force_pull = true
        ports = ["http", "grpc"]
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
