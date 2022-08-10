// https://discuss.hashicorp.com/t/upstream-connection-timeout-does-not-work/32078/5
// https://www.consul.io/docs/connect/config-entries/service-defaults
job "counter-api" {
  datacenters = ["dc1"]

  group "counter-api" {
    count = 1
    
    network {
      mode = "bridge"

      port "http" { 
        to = 5002 
      }
    }

    service {
      name = "counter-api"
      port = "5002"

      connect {
        sidecar_service {
          proxy {
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
        "traefik.port=5002",
        "traefik.http.routers.counterapi.entryPoints=web",
        "traefik.http.routers.counterapi.rule=Host(`nomadvn.eastus.cloudapp.azure.com`) && PathPrefix(`/counter-api`)",
        "traefik.http.routers.counterapi.middlewares=counterapi-stripprefix",
        "traefik.http.middlewares.counterapi-stripprefix.stripprefix.prefixes=/counter-api",
      ]
    }

    task "counter-api" {
      driver = "docker"

      config {
        image = "ghcr.io/thangchung/coffeeshop-on-nomad/counter-service:0.1.1"
        ports = [ "http" ]
        // force_pull = true
      }

      env {
        ASPNETCORE_ENVIRONMENT = "Development"
        RestPort = "${NOMAD_PORT_http}"
        ProductUri = "http://${NOMAD_IP_http}:15001"
        ConnectionStrings__counterdb = "Server=${attr.unique.network.ip-address};Port=5432;Database=postgres;User Id=postgres;Password=P@ssw0rd"
        RabbitMqUrl = "${attr.unique.network.ip-address}"
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
