using System;
namespace Shared.Events
{
	public class PaymentCompletedEvent
	{
		public Guid OrderId { get; set; }
	}
}

