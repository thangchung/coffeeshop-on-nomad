using CoffeeShop.Protobuf.Item.V1;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using ProductService.Domain;

namespace ProductService.Infrastructure.Grpc;

public class ItemService : ItemApi.ItemApiBase
{
    public override Task<Empty> Ping(Empty request, ServerCallContext context)
    {
        return Task.FromResult(new Empty());
    }

    public override Task<GetItemTypesResponse> GetItemTypes(GetItemTypesRequest request, ServerCallContext context)
    {
        var response = new GetItemTypesResponse();
        // beverages
        response.ItemTypes.Add(new ItemTypeDto { Name = ItemType.CAPPUCCINO.ToString(), ItemType = (int)ItemType.CAPPUCCINO });
        response.ItemTypes.Add(new ItemTypeDto { Name = ItemType.COFFEE_BLACK.ToString(), ItemType = (int)ItemType.COFFEE_BLACK });
        response.ItemTypes.Add(new ItemTypeDto { Name = ItemType.COFFEE_WITH_ROOM.ToString(), ItemType = (int)ItemType.COFFEE_WITH_ROOM });
        response.ItemTypes.Add(new ItemTypeDto { Name = ItemType.ESPRESSO.ToString(), ItemType = (int)ItemType.ESPRESSO });
        response.ItemTypes.Add(new ItemTypeDto { Name = ItemType.ESPRESSO_DOUBLE.ToString(), ItemType = (int)ItemType.ESPRESSO_DOUBLE });
        response.ItemTypes.Add(new ItemTypeDto { Name = ItemType.LATTE.ToString(), ItemType = (int)ItemType.LATTE });
        // food
        response.ItemTypes.Add(new ItemTypeDto { Name = ItemType.CAKEPOP.ToString(), ItemType = (int)ItemType.CAKEPOP });
        response.ItemTypes.Add(new ItemTypeDto { Name = ItemType.CROISSANT.ToString(), ItemType = (int)ItemType.CROISSANT });
        response.ItemTypes.Add(new ItemTypeDto { Name = ItemType.MUFFIN.ToString(), ItemType = (int)ItemType.MUFFIN });
        response.ItemTypes.Add(new ItemTypeDto { Name = ItemType.CROISSANT_CHOCOLATE.ToString(), ItemType = (int)ItemType.CROISSANT_CHOCOLATE });
        return Task.FromResult(response);
    }

    public override Task<GetItemsByTypesResponse> GetItemsByIds(GetItemsByTypesRequest request, ServerCallContext context)
    {
        var response = new GetItemsByTypesResponse();
        var itemTypes = request.ItemTypes.Split(",").Select(id => (ItemType)Convert.ToInt16(id));
        foreach (var itemType in itemTypes)
        {
            var temp = Domain.Item.GetItem(itemType);
            response.Items.Add(new ItemDto { Type = (int)temp.Type, Price = (double)temp.Price });
        }
        return Task.FromResult(response);
    }
}
