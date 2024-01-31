using Shared.Messages;

namespace Shared.Events
{
    public class OrderCreatedEvent
	{
		public Guid OrderId { get; set; }
		public Guid BuyerId { get; set; }
		public Decimal TotalPrice { get; set; }
		public List<OrderItemMessage> OrderItems { get; set; }
	}
}

