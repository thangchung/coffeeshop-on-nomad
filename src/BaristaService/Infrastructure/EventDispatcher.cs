using BaristaService.Domain.DomainEvents;
using CoffeeShop.Contracts;
using MassTransit;
using MediatR;
using N8T.Core.Domain;

namespace BaristaService.Infrastructure;

public class EventDispatcher : INotificationHandler<EventWrapper>
{
    private readonly IPublishEndpoint _publisher;

    public EventDispatcher(IPublishEndpoint publisher)
    {
        _publisher = publisher;
    }

    public virtual async Task Handle(EventWrapper @eventWrapper, CancellationToken cancellationToken)
    {
        if (@eventWrapper.Event is BaristaOrderUp baristaOrderUpEvent)
        {
            await _publisher.Publish<BaristaOrderUpdated>(new
            {
                baristaOrderUpEvent.OrderId,
                baristaOrderUpEvent.ItemLineId,
                baristaOrderUpEvent.Name,
                baristaOrderUpEvent.ItemType,
                baristaOrderUpEvent.MadeBy,
                baristaOrderUpEvent.TimeUp
            }, cancellationToken);
        }
    }
}
