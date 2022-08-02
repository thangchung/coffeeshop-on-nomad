using BaristaService.Domain;
using CoffeeShop.Contracts;
using MassTransit;
using MediatR;
using N8T.Core.Domain;
using N8T.Core.Repository;

namespace BaristaService.Consumers;

public class BaristaOrderedConsumer : IConsumer<BaristaOrdered>
{
    private readonly IRepository<BaristaItem> _baristaItemRepository;
    private readonly IPublisher _publisher;

    public BaristaOrderedConsumer(IRepository<BaristaItem> baristaItemRepository, IPublisher publisher)
    {
        _baristaItemRepository = baristaItemRepository;
        _publisher = publisher;
    }

    public async Task Consume(ConsumeContext<BaristaOrdered> context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var message = context.Message;
        var baristaItem = BaristaItem.From(message.ItemType, message.ItemType.ToString(), DateTime.UtcNow);

        await Task.Delay(CalculateDelay(message.ItemType));

        baristaItem.SetTimeUp(message.OrderId, message.ItemLineId, DateTime.UtcNow);

        await _baristaItemRepository.AddAsync(baristaItem);

        await baristaItem.RelayAndPublishEvents(_publisher);
    }

    private static TimeSpan CalculateDelay(ItemType itemType)
    {
        return itemType switch
        {
            ItemType.COFFEE_BLACK => TimeSpan.FromSeconds(5),
            ItemType.COFFEE_WITH_ROOM => TimeSpan.FromSeconds(5),
            ItemType.ESPRESSO => TimeSpan.FromSeconds(7),
            ItemType.ESPRESSO_DOUBLE => TimeSpan.FromSeconds(7),
            ItemType.CAPPUCCINO => TimeSpan.FromSeconds(10),
            _ => TimeSpan.FromSeconds(3)
        };
    }
}

internal class BaristaOrderedConsumerDefinition : ConsumerDefinition<BaristaOrderedConsumer>
{
    public BaristaOrderedConsumerDefinition()
    {
        // override the default endpoint name
        EndpointName = "barista-service";

        // limit the number of messages consumed concurrently
        // this applies to the consumer only, not the endpoint
        ConcurrentMessageLimit = 8;
    }

    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<BaristaOrderedConsumer> consumerConfigurator)
    {
        // configure message retry with millisecond intervals
        endpointConfigurator.UseMessageRetry(r => r.Intervals(100, 200, 500, 800, 1000));

        // use the outbox to prevent duplicate events from being published
        endpointConfigurator.UseInMemoryOutbox();
    }
}
