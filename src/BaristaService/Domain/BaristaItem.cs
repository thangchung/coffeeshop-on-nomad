using BaristaService.Domain.DomainEvents;
using CoffeeShop.Contracts;
using N8T.Core.Domain;

namespace BaristaService.Domain;

public class BaristaItem : EntityRootBase
{
    public ItemType ItemType { get; }
    public string ItemName { get; } = null!;
    public DateTime TimeIn { get; }
    public DateTime TimeUp { get; private set; }

    private BaristaItem()
    {
        // for MediatR binding
    }

    private BaristaItem(ItemType itemType, string itemName, DateTime timeIn)
    {
        ItemType = itemType;
        ItemName = itemName;
        TimeIn = timeIn;
    }

    public static BaristaItem From(ItemType itemType, string itemName, DateTime timeIn)
    {
        return new BaristaItem(itemType, itemName, timeIn);
    }

    public BaristaItem SetTimeUp(Guid orderId, Guid itemLineId, DateTime timeUp)
    {
        AddDomainEvent(new BaristaOrderUp(orderId, itemLineId, ItemName, ItemType, DateTime.UtcNow, "teesee"));
        TimeUp = timeUp;
        return this;
    }
}
