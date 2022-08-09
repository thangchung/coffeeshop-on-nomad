job "kitchen-api" {
  datacenters = ["dc1"]

  group "kitchen-api" {
    count = 1
    
    network {
      mode = "bridge"

      port "http" {}
    }

    service {
      name = "kitchen-api"
      port = "5004"
    }

    task "kitchen-api" {
      driver = "docker"

      config {
        image = "ghcr.io/thangchung/coffeeshop-on-nomad/kitchen-service:0.1.1"
        // force_pull = true
      }

      env {
        ASPNETCORE_ENVIRONMENT = "Development"
        RestPort = "5004"
        ConnectionStrings__kitchendb = "Server=${NOMAD_IP_http};Port=5432;Database=postgres;User Id=postgres;Password=P@ssw0rd"
        RabbitMqUrl = "${NOMAD_IP_http}"
        UseTracingExporter = "console1"
        UseMetricsExporter = "console1"
        UseLogExporter = "console1"
        AspNetCoreInstrumentation__RecordException = true
      }

      resources {
        cpu    = 100
        memory = 200
      }
    }
  }
}
