job "product-api" {
  datacenters = ["dc1"]

  group "svc" {
    count = 1
    
    network {
      mode = "bridge"

      port "http" {
        to = 5001
      }
    }

    service {
      name = "product-api-http"
      port = "5001"

      connect {
        sidecar_service {}
      }

      tags = [
        "traefik.enable=true",
        "traefik.consulcatalog.connect=true",
        "traefik.port=5001",
        "traefik.http.routers.productapi.entryPoints=web",
        "traefik.http.routers.productapi.rule=Host(`nomadvn.eastus.cloudapp.azure.com`) && PathPrefix(`/product-api`)",
        "traefik.http.routers.productapi.middlewares=productapi-stripprefix",
        "traefik.http.middlewares.productapi-stripprefix.stripprefix.prefixes=/product-api",
      ]
    }

    task "product-api" {
      driver = "docker"

      // artifact {
      //   source = "git::https://github.com/thangchung/coffeeshop-on-nomad"
      //   destination = "local/repo"
      // }

      // config {
      //   command = "bash"
      //   args = [
      //     "-c",
      //     "cd local/repo/ && export DOTNET_ROOT=/usr/share/dotnet && export PATH=$PATH:/usr/share/dotnet && dotnet build ./src/ProductService/ProductService.csproj && ./src/ProductService/bin/Debug/net7.0/ProductService"
      //   ]
      // }

      config {
        image = "ghcr.io/thangchung/coffeeshop-on-nomad/product-service:0.1.3"
        // force_pull = true
        ports = ["http"]
      }

      env {
        ASPNETCORE_ENVIRONMENT = "Development"
        COREHOST_TRACE = 1
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
