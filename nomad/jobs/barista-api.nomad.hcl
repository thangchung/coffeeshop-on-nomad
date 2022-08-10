job "barista-api" {
  datacenters = ["dc1"]

  group "barista-api" {
    count = 1
    
    network {
      mode = "bridge"

      port "http" { 
        to = 5003 
      }
    }

    service {
      name = "barista-api"
      port = "5003"
      address = "${attr.unique.network.ip-address}"
    }

    task "barista-api" {
      driver = "docker"

      config {
        image = "ghcr.io/thangchung/coffeeshop-on-nomad/barista-service:0.1.1"
        // force_pull = true
        ports = [ "http" ]
      }

      env {
        ASPNETCORE_ENVIRONMENT = "Development"
        RestPort = "5003"
        ConnectionStrings__baristadb = "Server=${attr.unique.network.ip-address};Port=5432;Database=postgres;User Id=postgres;Password=P@ssw0rd"
        RabbitMqUrl = "${attr.unique.network.ip-address}"
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
