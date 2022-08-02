namespace CoffeeShop.Contracts;

public interface KitchenOrdered
{
    public Guid OrderId { get; }
    public Guid ItemLineId { get; }
    public ItemType ItemType { get; }
}
