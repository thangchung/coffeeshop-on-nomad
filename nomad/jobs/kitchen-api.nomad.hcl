job "kitchen-api" {
  datacenters = ["dc1"]

  group "kitchen-api" {
    count = 1
    
    network {
      mode = "bridge"

      port "kitchen_api_http" {}
    }

    service {
      name = "kitchen-api"
      port = "kitchen_api_http"

      connect {
        sidecar_service { }
      }
    }

    task "kitchen-api" {
      driver = "docker"

      config {
        image = "ghcr.io/thangchung/coffeeshop-on-nomad/kitchen-service:0.1.0"
        // force_pull = true
      }

      env {
        ASPNETCORE_ENVIRONMENT = "Development"
        RestPort = "${NOMAD_PORT_kitchen_api_http}"
        ConnectionStrings__kitchendb = "Server=${NOMAD_IP_kitchen_api_http};Port=5432;Database=postgres;User Id=postgres;Password=P@ssw0rd"
        RabbitMqUrl = "${NOMAD_IP_kitchen_api_http}"
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
