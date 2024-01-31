using System;
using MassTransit;
using Shared.Events;

namespace Payment.API.Consumer
{
	public class StockReservedEventConsumer : IConsumer<StockReservedEvent>
	{
        readonly IPublishEndpoint _publishEndpoint;

        public StockReservedEventConsumer(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<StockReservedEvent> context)
        {
            if (true)
            {
                PaymentCompletedEvent paymentCompletedEvent = new PaymentCompletedEvent
                {
                    OrderId = context.Message.OrderId
                };

                await _publishEndpoint.Publish(paymentCompletedEvent);
            }
            else
            {
                PaymentFailedEvent paymentFailedEvent = new PaymentFailedEvent
                {
                    OrderId = context.Message.OrderId,
                    Message = "Kart bilgileri hatalı",
                    OrderItems = context.Message.OrderItems
                };

                await _publishEndpoint.Publish(paymentFailedEvent);
            }
        }
    }
}

