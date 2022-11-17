using CounterService.Domain;
using CounterService.Domain.Commands;
using FluentValidation;
using MediatR;
using N8T.Core.Domain;
using N8T.Core.Repository;

namespace CounterService.Features;

public static class OrderInRouteMapper
{
    public static IEndpointRouteBuilder MapOrderInApiRoutes(this IEndpointRouteBuilder builder)
    {
        builder.MapPost("", async (PlaceOrderCommand command, ISender sender) => await sender.Send(command));
        return builder;
    }
}

internal class OrderInValidator : AbstractValidator<PlaceOrderCommand>
{
}

internal class PlaceOrderHandler : IRequestHandler<PlaceOrderCommand, IResult>
{
    private readonly IRepository<Order> _orderRepository;
    private readonly IItemGateway _itemGateway;
    private readonly IPublisher _publisher;

    public PlaceOrderHandler(IRepository<Order> orderRepository, IItemGateway itemGateway, IPublisher publisher)
    {
        _orderRepository = orderRepository;
        _itemGateway = itemGateway;
        _publisher = publisher;
    }

    public async Task<IResult> Handle(PlaceOrderCommand placeOrderCommand, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(placeOrderCommand);

        var order = await Order.From(placeOrderCommand, _itemGateway);
        await _orderRepository.AddAsync(order, cancellationToken: cancellationToken);

        await order.RelayAndPublishEvents(_publisher, cancellationToken);

        return Results.Ok();
    }
}
