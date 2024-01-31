using System;
using Order.API.Enums;

namespace Order.API.Models
{
	public class Order
	{
		public Guid Id { get; set; }
		public Guid BuyerId { get; set; }
		public List<OrderItem> OrderItems { get; set; }
		public OrderStatus OrderStatus { get; set; }
		public DateTime CreateDate { get; set; }
		public decimal TotalPrice { get; set; }
	}
}

