using CoffeeShop.Contracts;

namespace CounterService.Domain;

public interface IItemGateway
{
    Task<IEnumerable<ItemDto>> GetItemsByType(ItemType[] itemTypes);
}