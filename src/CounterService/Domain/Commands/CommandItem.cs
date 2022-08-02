using CoffeeShop.Contracts;

namespace CoffeeShop.Domain.Commands;

// Representst the individual line items in a PlaceOrderCommand
public class CommandItem
{
    public ItemType ItemType { get; set; }
}
