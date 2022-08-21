job "datagen" {
  datacenters = ["dc1"]

  constraint {
    attribute = "${attr.kernel.name}"
    value     = "linux"
  }

  group "datagen" {
    count = 1
    
    network {
      mode = "bridge"

      port "http" { }
    }

    service {
      name = "datagen"

      connect {
        sidecar_service {}
      }
    }

    task "datagen" {
      driver = "raw_exec"

      artifact {
        source = "git::https://github.com/thangchung/coffeeshop-on-nomad"
        destination = "local/repo"
      }

      config {
        command = "bash"
        args = [
          "-c",
          "cd local/repo/ && dotnet build ./experiments/grpc-dns/src/Client/DataGen.csproj && dotnet run ./experiments/grpc-dns/src/Client/DataGen.csproj"
        ]
      }

      env {
        ASPNETCORE_ENVIRONMENT = "Development"
        UseGrpcDns = true
        ConsulServerUri = "dns:///127.0.0.1:8500/server-api.service.consul"
      }

      resources {
        cpu    = 70
        memory = 150
      }
    }
  }
}
