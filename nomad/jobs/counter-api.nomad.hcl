// https://discuss.hashicorp.com/t/upstream-connection-timeout-does-not-work/32078/5
// https://www.consul.io/docs/connect/config-entries/service-defaults
job "counter-api" {
  datacenters = ["dc1"]

  group "counter-api" {
    count = 1
    
    network {
      mode = "bridge"

      port "http" {}
    }

    service {
      name = "counter-api"
      port = "5002"

      connect {
        sidecar_service { }
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
        image = "ghcr.io/thangchung/coffeeshop-on-nomad/counter-service:0.1.1"
        // force_pull = true
      }

      env {
        ASPNETCORE_ENVIRONMENT = "Development"
        RestPort = "5002"
        ProductUri = "http://${NOMAD_IP_http}:15001"
        ConnectionStrings__counterdb = "Server=${NOMAD_IP_http};Port=5432;Database=postgres;User Id=postgres;Password=P@ssw0rd"
        RabbitMqUrl = "${NOMAD_IP_http}"
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
