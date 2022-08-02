namespace CoffeeShop.Contracts;

public interface KitchenOrderUpdated
{
    public Guid OrderId { get; }
    public Guid ItemLineId { get; }
    public string Name { get; }
    public ItemType ItemType { get; }
    public DateTime TimeIn { get; }
    public string MadeBy { get; }
    public DateTime TimeUp { get; }
}
