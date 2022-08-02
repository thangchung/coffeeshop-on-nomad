using CoffeeShop.Contracts;
using N8T.Core.Domain;

namespace CounterService.Domain;

public class LineItem : EntityBase
{
    public ItemType ItemType { get; set; }
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public ItemStatus ItemStatus { get; set; }
    public bool IsBaristaOrder { get; set; }
    
    public LineItem()
    {
    }

    public LineItem(ItemType itemType, string name, decimal price, ItemStatus itemStatus, bool isBarista)
    {
        ItemType = itemType;
        Name = name;
        Price = price;
        ItemStatus = itemStatus;
        IsBaristaOrder = isBarista;
    }
}
