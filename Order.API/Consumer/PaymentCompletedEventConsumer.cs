using System;
using MassTransit;
using Order.API.Models.Contexts;
using Shared.Events;

namespace Order.API.Consumer
{
    public class PaymentCompletedEventConsumer : IConsumer<PaymentCompletedEvent>
    {
        readonly OrderApiContext _contex;

        public PaymentCompletedEventConsumer(OrderApiContext contex)
        {
            _contex = contex;
        }

        public async Task Consume(ConsumeContext<PaymentCompletedEvent> context)
        {
            var order = await _contex.Orders.FindAsync(context.Message.OrderId);
            order.OrderStatus = Enums.OrderStatus.Completed;

            await _contex.SaveChangesAsync();
        }
    }
}

