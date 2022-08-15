using CoffeeShop.Contracts;
using FluentValidation;
using MediatR;

namespace ProductService.Features;

internal static class ItemTypesQueryRouter
{
    public static IEndpointRouteBuilder MapItemsByIdsQueryApiRoutes(this IEndpointRouteBuilder builder)
    {
        builder.MapGet("/v1/api/item-types",
            async (ISender sender) =>
                await sender.Send(new ItemTypesQuery()));
        return builder;
    }
}

public record ItemTypesQuery : IRequest<IEnumerable<ItemTypeDto>>;

internal class ItemTypesQueryValidator : AbstractValidator<ItemTypesQuery>
{
}

internal class ItemTypesQueryHandler : IRequestHandler<ItemTypesQuery, IEnumerable<ItemTypeDto>>
{
    private readonly ILogger<ItemTypesQueryHandler> _logger;

    public ItemTypesQueryHandler(ILogger<ItemTypesQueryHandler> logger)
    {
        _logger = logger;
    }
    
    public Task<IEnumerable<ItemTypeDto>> Handle(ItemTypesQuery request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var results = new List<ItemTypeDto>
        {
            // beverages
            new() {Name = ItemType.CAPPUCCINO.ToString(), Type = ItemType.CAPPUCCINO},
            new() {Name = ItemType.COFFEE_BLACK.ToString(), Type = ItemType.COFFEE_BLACK},
            new() {Name = ItemType.COFFEE_WITH_ROOM.ToString(), Type = ItemType.COFFEE_WITH_ROOM},
            new() {Name = ItemType.ESPRESSO.ToString(), Type = ItemType.ESPRESSO},
            new() {Name = ItemType.ESPRESSO_DOUBLE.ToString(), Type = ItemType.ESPRESSO_DOUBLE},
            new() {Name = ItemType.LATTE.ToString(), Type = ItemType.LATTE},
            // food
            new() {Name = ItemType.CAKEPOP.ToString(), Type = ItemType.CAKEPOP},
            new() {Name = ItemType.CROISSANT.ToString(), Type = ItemType.CROISSANT},
            new() {Name = ItemType.MUFFIN.ToString(), Type = ItemType.MUFFIN},
            new() {Name = ItemType.CROISSANT_CHOCOLATE.ToString(), Type = ItemType.CROISSANT_CHOCOLATE}
        };

        return Task.FromResult(results.Distinct());
    }
}
