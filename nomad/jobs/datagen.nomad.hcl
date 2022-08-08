job "datagen" {
  datacenters = ["dc1"]

  group "datagen" {
    count = 1
    
    network {
      mode = "bridge"
    }

    service {
      name = "datagen"

      connect {
        sidecar_service { 
          proxy {
            upstreams {
              destination_name = "counter-api"
              local_bind_port  = 5002
            }
          }
        }
      }
    }

    task "datagen" {
      driver = "docker"

      config {
        image = "ghcr.io/thangchung/coffeeshop-on-nomad/datagen-app:0.1.1"
        // force_pull = true
      }

      env {
        ASPNETCORE_ENVIRONMENT = "Development"
        CoffeeShopApi = "http://${NOMAD_UPSTREAM_ADDR_counter_api}"
        SubmitOrderRoute = "/v1/api/orders"
      }

      resources {
        cpu    = 150
        memory = 200
      }
    }
  }
}
