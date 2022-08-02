using BaristaService.Domain.DomainEvents;
using CoffeeShop.Contracts;
using MassTransit;
using MediatR;
using N8T.Core.Domain;

namespace KitchenService.Infrastructure;

public class EventDispatcher : INotificationHandler<EventWrapper>
{
    private readonly IPublishEndpoint _publisher;

    public EventDispatcher(IPublishEndpoint publisher)
    {
        _publisher = publisher;
    }

    public virtual async Task Handle(EventWrapper @eventWrapper, CancellationToken cancellationToken)
    {
        if (@eventWrapper.Event is KitchenOrderUp kitchenOrderUpEvent)
        {
            await _publisher.Publish<KitchenOrderUpdated>(new
            {
                kitchenOrderUpEvent.OrderId,
                kitchenOrderUpEvent.ItemLineId,
                kitchenOrderUpEvent.Name,
                kitchenOrderUpEvent.ItemType,
                kitchenOrderUpEvent.MadeBy,
                kitchenOrderUpEvent.TimeUp
            }, cancellationToken);
        }
    }
}
