using System;
namespace Shared
{
	public static class RabbitMQSettings
	{
		public const string Stock_OrderCreatedEventQueue = "stock-order-created-event-queue";
		public const string Payment_StockReservedEventQueue = "payment-stock-reserved-event-queue";
		public const string Order_PaymentCompletedEventQueue = "order-payment-completed-event-queue";
		public const string Order_PaymentFailedEventQueue = "order-payment-failed-event-queue";
		
	}
}

