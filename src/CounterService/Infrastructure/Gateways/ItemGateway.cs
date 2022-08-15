using CoffeeShop.Contracts;
using CounterService.Domain;

namespace CounterService.Infrastructure.Gateways;

public class ItemRestGateway : IItemGateway
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _config;
    private readonly ILogger<ItemRestGateway> _logger;

    public ItemRestGateway(IHttpClientFactory httpClientFactory, IConfiguration config, ILogger<ItemRestGateway> logger)
    {
        _httpClientFactory = httpClientFactory;
        _config = config;
        _logger = logger;
    }
    
    public async Task<IEnumerable<ItemDto>> GetItemsByType(ItemType[] itemTypes)
    {
        _logger.LogInformation("Start to call GetItemsByIdsAsync in Product Api");
        
        var httpClient = _httpClientFactory.CreateClient();
        httpClient.BaseAddress = new Uri(_config.GetValue<string>("ProductApiUri", "http://localhost:5001")!);
        
        var httpResponseMessage = await httpClient.GetFromJsonAsync<List<ItemDto>>(_config.GetValue<string>("GetItemTypesApiRoute", "/v1/api/item-types"));
        _logger.LogInformation("Can get {Count} items", httpResponseMessage?.Count);
        return httpResponseMessage ?? new List<ItemDto>();
    }
}

// public class ItemGrpcGateway : IItemGateway
// {
//     private readonly ItemApi.ItemApiClient _itemClient;
//     private readonly ILogger<ItemGrpcGateway> _logger;
//
//     public ItemGrpcGateway(GrpcClientFactory grpcClientFactory, ILogger<ItemGrpcGateway> logger)
//     {
//         _itemClient = grpcClientFactory.CreateClient<ItemApi.ItemApiClient>("ItemClient");
//         _logger = logger;
//     }
//
//     public async Task<IEnumerable<ItemDto>> GetItemsByType(ItemType[] itemTypes)
//     {
//         _logger.LogInformation("Start to call GetItemsByIdsAsync in Product Api");
//         var response = await _itemClient.GetItemsByIdsAsync(new GetItemsByTypesRequest { ItemTypes = itemTypes.Aggregate("", (a, b) => { return a + "," + (int)b; }).TrimStart(',') });
//         _logger.LogInformation("Can get {Count} items", response.Items.Count);
//         return response.Items;
//     }
// }
