using DataGen;
using Grpc.Net.Client.Balancer;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton<ResolverFactory>(
            sp => new DnsResolverFactory(refreshInterval: TimeSpan.FromSeconds(30)));
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();