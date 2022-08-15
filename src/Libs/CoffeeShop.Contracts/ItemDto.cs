namespace CoffeeShop.Contracts;

public class ItemDto
{
    public decimal Price { get; set; }
    public ItemType Type { get; set; }
}

public class ItemTypeDto
{
    public ItemType Type { get; set; }
    public string Name { get; set; }
}