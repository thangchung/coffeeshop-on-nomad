using CoffeeShop.Contracts;
using CoffeeShop.Domain.DomainEvents;
using MassTransit;
using MediatR;
using N8T.Core.Domain;

namespace CounterService.Infrastructure;

public class EventDispatcher : INotificationHandler<EventWrapper>
{
    private readonly IPublishEndpoint _publisher;

    public EventDispatcher(IPublishEndpoint publisher)
    {
        _publisher = publisher;
    }

    public virtual async Task Handle(EventWrapper @eventWrapper, CancellationToken cancellationToken)
    {
        if (@eventWrapper.Event is BaristaOrderIn baristaOrderInEvent)
        {
            await _publisher.Publish<BaristaOrdered>(new
            {
                baristaOrderInEvent.OrderId,
                baristaOrderInEvent.ItemLineId,
                baristaOrderInEvent.ItemType
            }, cancellationToken);
        }
        else if (@eventWrapper.Event is KitchenOrderIn kitchenOrderInEvent)
        {
            await _publisher.Publish<KitchenOrdered>(new
            {
                kitchenOrderInEvent.OrderId,
                kitchenOrderInEvent.ItemLineId,
                kitchenOrderInEvent.ItemType
            }, cancellationToken);
        }
    }
}
