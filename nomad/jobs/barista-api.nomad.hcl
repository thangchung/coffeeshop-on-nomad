job "barista-api" {
  datacenters = ["dc1"]

  group "barista-api" {
    count = 1
    
    network {
      mode = "bridge"

      port "barista_api_http" {}
    }

    service {
      name = "barista-api"
      port = "barista_api_http"

      connect {
        sidecar_service { }
      }
    }

    task "barista-api" {
      driver = "docker"

      config {
        image = "ghcr.io/thangchung/coffeeshop-on-nomad/barista-service:0.1.0"
        // force_pull = true
      }

      env {
        ASPNETCORE_ENVIRONMENT = "Development"
        RestPort = "${NOMAD_PORT_barista_api_http}"
        ConnectionStrings__baristadb = "Server=${NOMAD_IP_barista_api_http};Port=5432;Database=postgres;User Id=postgres;Password=P@ssw0rd"
        RabbitMqUrl = "${NOMAD_IP_barista_api_http}"
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
