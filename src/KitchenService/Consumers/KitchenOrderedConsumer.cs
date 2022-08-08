using CoffeeShop.Contracts;
using KitchenService.Domain;
using MassTransit;
using MediatR;
using N8T.Core.Domain;
using N8T.Core.Repository;

namespace KitchenService.Consumers;

public class KitchenOrderedConsumer : IConsumer<KitchenOrdered>
{
    private readonly IRepository<KitchenOrder> _kitchenOrderRepository;
    private readonly IPublisher _publisher;

    public KitchenOrderedConsumer(IRepository<KitchenOrder> kitchenOrderRepository, IPublisher publisher)
    {
        _kitchenOrderRepository = kitchenOrderRepository;
        _publisher = publisher;
    }

    public async Task Consume(ConsumeContext<KitchenOrdered> context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var message = context.Message;
        var kitchenOrder = KitchenOrder.From(message.OrderId, message.ItemType, DateTime.UtcNow);

        await Task.Delay(CalculateDelay(message.ItemType));

        kitchenOrder.SetTimeUp(message.ItemLineId, DateTime.UtcNow);

        await _kitchenOrderRepository.AddAsync(kitchenOrder);

        await kitchenOrder.RelayAndPublishEvents(_publisher);
    }

    private static TimeSpan CalculateDelay(ItemType itemType)
    {
        return itemType switch
        {
            ItemType.CROISSANT => TimeSpan.FromSeconds(7),
            ItemType.CROISSANT_CHOCOLATE => TimeSpan.FromSeconds(7),
            ItemType.CAKEPOP => TimeSpan.FromSeconds(5),
            ItemType.MUFFIN => TimeSpan.FromSeconds(7),
            _ => TimeSpan.FromSeconds(3)
        };
    }
}

internal class KitchenOrderedConsumerDefinition : ConsumerDefinition<KitchenOrderedConsumer>
{
    public KitchenOrderedConsumerDefinition()
    {
        // override the default endpoint name
        EndpointName = "kitchen-service";

        // limit the number of messages consumed concurrently
        // this applies to the consumer only, not the endpoint
        ConcurrentMessageLimit = 8;
    }

    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<KitchenOrderedConsumer> consumerConfigurator)
    {
        // configure message retry with millisecond intervals
        endpointConfigurator.UseMessageRetry(r => r.Intervals(100, 200, 500, 800, 1000));

        // use the outbox to prevent duplicate events from being published
        endpointConfigurator.UseInMemoryOutbox();
    }
}
