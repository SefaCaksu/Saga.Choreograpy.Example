using System;
using MassTransit;
using MongoDB.Driver;
using Shared.Events;
using Stock.API.Services;

namespace Stock.API.Consumers
{
    public class PaymentFailEventConsumer : IConsumer<PaymentFailedEvent>
    {
        readonly MongoDBService _mongoDBService;
        public PaymentFailEventConsumer(MongoDBService mongoDBService)
        {
            _mongoDBService = mongoDBService;
        }

        public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
        {
            var stocks = _mongoDBService.GetCollection<Models.Stock>();
            foreach (var orderItem in context.Message.OrderItems)
            {
                var stock = await (
                     await stocks.FindAsync(c => c.ProductId == orderItem.ProductId.ToString())
                     ).FirstOrDefaultAsync();

                stock.Count += orderItem.Count;

                await stocks.FindOneAndReplaceAsync(c => c.ProductId == orderItem.ProductId.ToString(), stock);
            }
        }
    }
}

