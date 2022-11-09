using Grpc.Core;

namespace ServerApi.Services;

public class GreeterService : Greeter.GreeterBase
{
    private readonly ILogger<GreeterService> _logger;

    public GreeterService(ILogger<GreeterService> logger)
    {
        _logger = logger;
    }

    public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
    {
        _logger.LogInformation("{ProcessName}-{ProcessId} Start to call to SayHello",
            Environment.MachineName,
            Environment.ProcessId);

        return Task.FromResult(new HelloReply
        {
            Message = "Hello " + request.Name
        });
    }
}