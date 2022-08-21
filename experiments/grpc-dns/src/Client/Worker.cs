using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Configuration;
using ServerApi;

namespace DataGen;

public class Worker : BackgroundService
{
    private readonly IConfiguration _config;
    private readonly ILogger<Worker> _logger;

    public Worker(IConfiguration config, ILogger<Worker> logger)
    {
        _config = config;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            // random to seed data into the system
            var rand = new Random();
            var timeUpPeriod = rand.Next(3, 5);
            //_logger.LogInformation("CoffeeShop URL: {url}", _config.GetValue<string>("CoffeeShopApi"));
            //_logger.LogInformation("Submit Order Route: {url}", _config.GetValue<string>("SubmitOrderRoute"));

            var useGrpcDns = _config.GetValue("UseGrpcDns", false);
            var chanOptions = new GrpcChannelOptions
            {
                Credentials = ChannelCredentials.Insecure,
            };

            var serverUri = "http://localhost:15000";
            if (useGrpcDns)
            {
                chanOptions.ServiceConfig = new ServiceConfig {LoadBalancingConfigs = {new RoundRobinConfig()}};
                serverUri = _config.GetValue<string>("ConsulServerUri");
            }

            using var channel = GrpcChannel.ForAddress(serverUri, chanOptions);

            var client = new Greeter.GreeterClient(channel);
            var reply = await client.SayHelloAsync(new HelloRequest {Name = "GreeterClient"},
                cancellationToken: stoppingToken);
            _logger.LogInformation("Reply: {Reply}", reply.Message);

            await Task.Delay(TimeSpan.FromSeconds(timeUpPeriod), stoppingToken);
        }
    }
}