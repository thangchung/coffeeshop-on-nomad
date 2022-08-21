job "server-api" {
  datacenters = ["dc1"]

  constraint {
    attribute = "${attr.kernel.name}"
    value     = "linux"
  }

  group "svc" {
    count = 2
    
    network {
      mode = "bridge"

      port "http" {
        to = 5001
      }
    }

    service {
      name = "server-api"
      port = "5001"

      connect {
        sidecar_service {}
      }
    }

    task "product-api" {
      driver = "raw_exec"

      artifact {
        source = "git::https://github.com/thangchung/coffeeshop-on-nomad"
        destination = "local/repo"
      }

      config {
        command = "bash"
        args = [
          "-c",
          "cd local/repo/ && dotnet build ./experiments/grpc-dns/src/ServerApi/ServerApi.csproj && dotnet run ./experiments/grpc-dns/src/ServerApi/ServerApi.csproj"
        ]
      }

      env {
        ASPNETCORE_ENVIRONMENT = "Development"
        COREHOST_TRACE = 1
        RestPort = 5000
        GrpcPort = 15000
      }

      resources {
        cpu    = 100
        memory = 128
      }
    }
  }
}
