job "counter-api" {
  datacenters = ["dc1"]

  group "counter-api" {
    count = 1
    
    network {
      mode = "bridge"
    }

    service {
      name = "counter-api"
      port = "5002"

      connect {
        sidecar_service {
          proxy {
            upstreams {
              destination_name = "product-api"
              local_bind_port  = 15001
            }
            upstreams {
              destination_name = "postgres-db"
              local_bind_port  = 5432
            }
            upstreams {
              destination_name = "rabbitmq"
              local_bind_port  = 5672
            }
          }
        }
      }

      tags = [
        "traefik.enable=true",
        "traefik.consulcatalog.connect=true",
        "traefik.http.routers.counterapi.rule=PathPrefix(`/counter-api`)",
        "traefik.http.routers.counterapi.middlewares=counterapi-stripprefix",
        "traefik.http.middlewares.counterapi-stripprefix.stripprefix.prefixes=/counter-api",
      ]
    }

    task "counter-api" {
      driver = "docker"

      config {
        image = "ghcr.io/thangchung/coffeeshop-on-nomad/counter-service:0.1.0"
        // force_pull = true
      }

      env {
        ASPNETCORE_ENVIRONMENT = "Development"
        RestPort = "5002"
        ProductUri = "http://${NOMAD_UPSTREAM_ADDR_product_api}"
        ConnectionStrings__counterdb = "Server=${NOMAD_UPSTREAM_IP_postgres_db};Port=5432;Database=postgres;User Id=postgres;Password=P@ssw0rd"
        RabbitMqUrl = "${NOMAD_UPSTREAM_IP_rabbitmq}"
        UseTracingExporter = "console1"
        UseMetricsExporter = "console1"
        UseLogExporter = "console1"
        AspNetCoreInstrumentation__RecordException = true
      }

      resources {
        cpu    = 150
        memory = 200
      }
    }
  }
}
