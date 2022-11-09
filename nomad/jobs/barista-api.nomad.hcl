variable "docker-image-version" {
  type        = string
  default     = "latest"
  description = "the docker image version"
}

job "barista-api" {
  datacenters = ["dc1"]

  constraint {
    attribute = "${attr.kernel.name}"
    value     = "linux"
  }

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
        image = "ghcr.io/thangchung/coffeeshop-on-nomad/barista-service:${var.docker-image-version}"
        // force_pull = true
        ports = [ "http" ]
      }

      env {
        ASPNETCORE_ENVIRONMENT = "Development"
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
