// using CoffeeShop.Protobuf.Item.V1;
// using Google.Protobuf.WellKnownTypes;
// using Grpc.Core;
// using MediatR;
// using ProductService.Features;
//
// namespace ProductService.Infrastructure.Grpc;
//
// public class ItemService : ItemApi.ItemApiBase
// {
//     private readonly ISender _sender;
//     private readonly ILogger<ItemService> _logger;
//
//     public ItemService(ISender sender, ILogger<ItemService> logger)
//     {
//         _sender = sender;
//         _logger = logger;
//     }
//     
//     public override Task<Empty> Ping(Empty request, ServerCallContext context)
//     {
//         _logger.LogInformation("Start to process Ping");
//         
//         return Task.FromResult(new Empty());
//     }
//
//     public override async Task<GetItemTypesResponse> GetItemTypes(GetItemTypesRequest request, ServerCallContext context)
//     {
//         _logger.LogInformation("Start to process GetItemTypes");
//         
//         var response = new GetItemTypesResponse();
//         var results = await _sender.Send(new ItemTypesQuery());
//         response.ItemTypes.AddRange(results);
//         
//         return response;
//     }
//
//     public override async Task<GetItemsByTypesResponse> GetItemsByIds(GetItemsByTypesRequest request, ServerCallContext context)
//     {
//         _logger.LogInformation("Start to process GetItemsByIds");
//         
//         var response = new GetItemsByTypesResponse();
//         var results = await _sender.Send(new ItemsByIdsQuery(request.ItemTypes));
//         response.Items.AddRange(results);
//         
//         return response;
//     }
// }
