using CoffeeShop.Contracts;
using N8T.Core.Domain;

namespace CounterService.Domain.DomainEvents;

public class OrderIn : EventBase
{
    public Guid OrderId { get; set; }
    public Guid ItemLineId { get; set; }
    public ItemType ItemType { get; set; }
    public DateTime TimeIn { get; set; }

    public OrderIn(Guid orderId, Guid itemLineId, ItemType itemType)
    {
        OrderId = orderId;
        ItemLineId = itemLineId;
        ItemType = itemType;
        TimeIn = DateTime.UtcNow;
    }
}

public class BaristaOrderIn : OrderIn
{
    public BaristaOrderIn(Guid orderId, Guid itemLineId, ItemType itemType) 
        : base(orderId, itemLineId, itemType)
    {
    }
}

public class KitchenOrderIn : OrderIn
{
    public KitchenOrderIn(Guid orderId, Guid itemLineId, ItemType itemType)
        : base(orderId, itemLineId, itemType)
    {
    }
}
