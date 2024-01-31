using System;
using MassTransit;
using Order.API.Models.Contexts;
using Shared.Events;

namespace Order.API.Consumer
{
	public class PaymentFailedEventConsumer : IConsumer<PaymentFailedEvent>
	{
        readonly OrderApiContext _contex;

        public PaymentFailedEventConsumer(OrderApiContext contex)
        {
            _contex = contex;
        }

        public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
        {
            var order = await _contex.Orders.FindAsync(context.Message.OrderId);
            order.OrderStatus = Enums.OrderStatus.Fail;

            await _contex.SaveChangesAsync();
        }
    }
}

