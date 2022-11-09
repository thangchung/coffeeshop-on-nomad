using CoffeeShop.Contracts;
using CounterService.Domain;
using CounterService.Domain.DomainEvents;
using MassTransit;
using MediatR;
using N8T.Core.Domain;
using N8T.Core.Repository;

namespace CounterService.Consumers;

internal class KitchenOrderUpdatedConsumer : IConsumer<KitchenOrderUpdated>
{
    private readonly IRepository<Order> _orderRepository;
    private readonly IPublisher _publisher;

    public KitchenOrderUpdatedConsumer(IRepository<Order> orderRepository, IPublisher publisher)
    {
        _orderRepository = orderRepository;
        _publisher = publisher;
    }

    public async Task Consume(ConsumeContext<KitchenOrderUpdated> context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var message = context.Message;
        var spec = new GetOrderByIdWithLineItemSpec(message.OrderId);
        var order = await _orderRepository.FindOneAsync(spec);

        var orderUpdated = order.Apply(
            new OrderUp(
                message.OrderId,
                message.ItemLineId,
                message.ItemType.ToString(),
                message.ItemType,
                message.TimeUp,
                message.MadeBy));

        await _orderRepository.EditAsync(orderUpdated);

        await order.RelayAndPublishEvents(_publisher);
    }
}

internal class KitchenOrderUpdatedConsumerDefinition : ConsumerDefinition<KitchenOrderUpdatedConsumer>
{
    public KitchenOrderUpdatedConsumerDefinition()
    {
        // override the default endpoint name
        EndpointName = "order-updated";

        // limit the number of messages consumed concurrently
        // this applies to the consumer only, not the endpoint
        ConcurrentMessageLimit = 1;
    }

    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<KitchenOrderUpdatedConsumer> consumerConfigurator)
    {
        // configure message retry with millisecond intervals
        endpointConfigurator.UseMessageRetry(r => r.Intervals(100, 200, 500, 800, 1000));

        // use the outbox to prevent duplicate events from being published
        endpointConfigurator.UseInMemoryOutbox();
    }
}
