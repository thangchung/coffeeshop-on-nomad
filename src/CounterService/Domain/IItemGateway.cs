using CoffeeShop.Contracts;
using CoffeeShop.Protobuf.Item.V1;

namespace CounterService.Domain;

public interface IItemGateway
{
    Task<IEnumerable<ItemDto>> GetItemsByType(ItemType[] itemTypes);
}