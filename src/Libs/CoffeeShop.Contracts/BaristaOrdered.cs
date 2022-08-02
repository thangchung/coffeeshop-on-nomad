namespace CoffeeShop.Contracts;

public interface BaristaOrdered
{
    public Guid OrderId { get; }
    public Guid ItemLineId { get; }
    public ItemType ItemType { get; }
}
