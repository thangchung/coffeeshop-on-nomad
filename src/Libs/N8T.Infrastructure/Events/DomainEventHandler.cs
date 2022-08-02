using System.Threading;
using System.Threading.Tasks;
using MediatR;
using N8T.Core.Domain;

namespace N8T.Infrastructure.Events;

public abstract class DomainEventHandler<TEvent> : INotificationHandler<EventWrapper>
    where TEvent : IDomainEvent
{
    public abstract Task HandleEvent(TEvent @event, CancellationToken cancellationToken);

    public virtual async Task Handle(EventWrapper @eventWrapper, CancellationToken cancellationToken)
    {
        if (@eventWrapper.Event is TEvent @event)
        {
            await HandleEvent(@event, cancellationToken);
        }
    }
}
