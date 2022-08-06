using CoffeeShop.Contracts;
using CoffeeShop.Protobuf.Item.V1;
using CounterService.Domain;
using Grpc.Net.ClientFactory;

namespace CounterService.Infrastructure.Gateways;

public class ItemGateway : IItemGateway
{
    private readonly ItemApi.ItemApiClient _itemClient;
    private readonly ILogger<ItemGateway> _logger;

    public ItemGateway(GrpcClientFactory grpcClientFactory, ILogger<ItemGateway> logger)
    {
        _itemClient = grpcClientFactory.CreateClient<ItemApi.ItemApiClient>("ItemClient");
        _logger = logger;
    }

    public async Task<IEnumerable<ItemDto>> GetItemsByType(ItemType[] itemTypes)
    {
        _logger.LogInformation("Start to call GetItemsByIdsAsync in Product Api");
        var response = await _itemClient.GetItemsByIdsAsync(new GetItemsByTypesRequest { ItemTypes = itemTypes.Aggregate("", (a, b) => { return a + "," + (int)b; }).TrimStart(',') });
        _logger.LogInformation("Can get {count} items", response.Items.Count);
        return response.Items;
    }
}
