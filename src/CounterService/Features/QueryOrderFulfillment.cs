using CounterService.Domain;
using FluentValidation;
using MediatR;
using N8T.Core.Repository;
using N8T.Core.Specification;
using System.Linq.Expressions;

namespace CounterService.Features;

internal static class OrderFulfillmentRouteMapper
{
    public static IEndpointRouteBuilder MapOrderFulfillmentApiRoutes(this IEndpointRouteBuilder builder)
    {
        builder.MapGet("/v1/api/fulfillment-orders", async (ISender sender) => await sender.Send(new OrderFulfillmentQuery()));
        return builder;
    }
}

public record OrderFulfillmentQuery : IRequest<IResult>
{
}

internal class OrderFulfillmentSpec : SpecificationBase<Order>
{
    public OrderFulfillmentSpec()
    {
        AddInclude(x => x.LineItems);
    }

    public override Expression<Func<Order, bool>> Criteria => x => x.OrderStatus == OrderStatus.FULFILLED;
}

internal class OrderFulfillmentValidator : AbstractValidator<OrderFulfillmentQuery>
{
    public OrderFulfillmentValidator()
    {
    }
}

internal class QueryOrderFulfillment : IRequestHandler<OrderFulfillmentQuery, IResult>
{
    private readonly IRepository<Order> _orderRepository;

    public QueryOrderFulfillment(
        IRepository<Order> orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<IResult> Handle(OrderFulfillmentQuery query, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);
        var orders = await _orderRepository.FindAsync(new OrderFulfillmentSpec(), cancellationToken);
        return Results.Ok(orders);
    }
}