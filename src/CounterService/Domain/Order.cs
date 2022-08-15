using CoffeeShop.Domain.Commands;
using CoffeeShop.Domain.DomainEvents;
using CounterService.Domain.DomainEvents;
using N8T.Core.Domain;

namespace CounterService.Domain;

public class Order : EntityRootBase
{
    public OrderSource OrderSource { get; }
    public Guid LoyaltyMemberId { get; }
    public OrderStatus OrderStatus { get; private set; }
    public Location Location { get; }
    public List<LineItem> LineItems { get; } = new();

    private Order()
    {
        // for MediatR binding    
    }

    private Order(OrderSource orderSource, Guid loyaltyMemberId, OrderStatus orderStatus, Location location)
    {
        OrderSource = orderSource;
        LoyaltyMemberId = loyaltyMemberId;
        OrderStatus = orderStatus;
        Location = location;
    }

    public static async Task<Order> From(PlaceOrderCommand placeOrderCommand, IItemGateway itemGateway)
    {
        var order = new Order(placeOrderCommand.OrderSource, placeOrderCommand.LoyaltyMemberId, OrderStatus.IN_PROGRESS, placeOrderCommand.Location);

        if (placeOrderCommand.BaristaItems.Any())
        {
            var itemTypes = placeOrderCommand.BaristaItems.Select(x => x.ItemType);
            var items = await itemGateway.GetItemsByType(itemTypes.ToArray());
            foreach (var baristaItem in placeOrderCommand.BaristaItems)
            {
                var item = items.FirstOrDefault(x => x.Type == baristaItem.ItemType);
                var lineItem = new LineItem(baristaItem.ItemType, item?.Type.ToString()!, (decimal)item?.Price!, ItemStatus.IN_PROGRESS, true);

                order.AddDomainEvent(new OrderUpdate(order.Id, lineItem.Id, lineItem.ItemType, OrderStatus.IN_PROGRESS));
                order.AddDomainEvent(new BaristaOrderIn(order.Id, lineItem.Id, lineItem.ItemType));

                order.LineItems.Add(lineItem);
            }
        }

        if (placeOrderCommand.KitchenItems.Any())
        {
            var itemTypes = placeOrderCommand.KitchenItems.Select(x => x.ItemType);
            var items = await itemGateway.GetItemsByType(itemTypes.ToArray());
            foreach (var kitchenItem in placeOrderCommand.KitchenItems)
            {
                var item = items.FirstOrDefault(x => x.Type == kitchenItem.ItemType);
                var lineItem = new LineItem(kitchenItem.ItemType, item?.Type.ToString()!, (decimal)item?.Price!, ItemStatus.IN_PROGRESS, false);

                order.AddDomainEvent(new OrderUpdate(order.Id, lineItem.Id, lineItem.ItemType, OrderStatus.IN_PROGRESS));
                order.AddDomainEvent(new KitchenOrderIn(order.Id, lineItem.Id, lineItem.ItemType));

                order.LineItems.Add(lineItem);
            }
        }

        return order;
    }

    public Order Apply(OrderUp orderUp)
    {
        if (!LineItems.Any()) return this;
        
        var item = LineItems.FirstOrDefault(i => i.Id == orderUp.ItemLineId);
        if (item is not null)
        {
            item.ItemStatus = ItemStatus.FULFILLED;
            AddDomainEvent(new OrderUpdate(Id, item.Id, item.ItemType, OrderStatus.FULFILLED, orderUp.MadeBy));
        }

        // if there are both barista and kitchen items is fulfilled then checking status and change order to Fulfilled
        if (LineItems.All(i => i.ItemStatus == ItemStatus.FULFILLED))
        {
            OrderStatus = OrderStatus.FULFILLED;
        }
        return this;
    }
}
