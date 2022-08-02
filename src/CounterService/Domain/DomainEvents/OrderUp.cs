using CoffeeShop.Contracts;
using N8T.Core.Domain;

namespace CoffeeShop.Domain.DomainEvents;

public class OrderUp : EventBase
{
    // OrderIn info
    public Guid OrderId { get; }
    public Guid ItemLineId { get; }
    public string Name { get; }
    public ItemType ItemType { get; }
    public DateTime TimeIn { get; }
    
    public string MadeBy { get; }
    public DateTime TimeUp { get; }

    public OrderUp(Guid orderId, Guid itemLineId, string name, ItemType itemType, DateTime timeUp, string madeBy)
    {
        OrderId = orderId;
        ItemLineId = itemLineId;
        Name = name;
        ItemType = itemType;
        TimeIn = DateTime.UtcNow;
        MadeBy = madeBy;
        TimeUp = timeUp;
    }

    public override void Flatten()
    {
        throw new NotImplementedException();
    }
}

public class BaristaOrderUp : OrderUp
{
    public BaristaOrderUp(Guid orderId, Guid itemLineId, string name, ItemType itemType, DateTime timeUp, string madeBy) 
        : base(orderId, itemLineId, name, itemType, timeUp, madeBy)
    {
    }
}

public class KitchenOrderUp : OrderUp
{
    public KitchenOrderUp(Guid orderId, Guid itemLineId, string name, ItemType itemType, DateTime timeUp, string madeBy)
        : base(orderId, itemLineId, name, itemType, timeUp, madeBy)
    {
    }
}
