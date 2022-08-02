using CoffeeShop.Contracts;
using CoffeeShop.Protobuf.Item.V1;
using CounterService.Domain;
using Grpc.Net.ClientFactory;

namespace CounterService.Infrastructure.Gateways;

public class ItemGateway : IItemGateway
{
    private readonly ItemApi.ItemApiClient _itemClient;

    public ItemGateway(GrpcClientFactory grpcClientFactory)
    {
        _itemClient = grpcClientFactory.CreateClient<ItemApi.ItemApiClient>("ItemClient");
    }

    public async Task<IEnumerable<ItemDto>> GetItemsByType(ItemType[] itemTypes)
    {
        var response = await _itemClient.GetItemsByIdsAsync(new GetItemsByTypesRequest { ItemTypes = itemTypes.Aggregate("", (a, b) => { return a + "," + (int)b; }).TrimStart(',') });
        return response.Items;
    }
}
