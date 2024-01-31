using MassTransit;
using MongoDB.Driver;
using Shared;
using Shared.Events;
using Stock.API.Services;

namespace Stock.API.Consumers
{
    public class OrderCreatedEventConsumer : IConsumer<OrderCreatedEvent>
    {
        readonly MongoDBService _mongoDbService;
        readonly ISendEndpointProvider _sendEndpointProvider;
        readonly IPublishEndpoint _publishEndPoint;



        public OrderCreatedEventConsumer(
            MongoDBService mongoDbService
            , ISendEndpointProvider sendEndpointProvider
            , IPublishEndpoint publishEndPoint)
        {
            _mongoDbService = mongoDbService;
            _sendEndpointProvider = sendEndpointProvider;
            _publishEndPoint = publishEndPoint;
        }

        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            List<bool> stockResult = new();
            IMongoCollection<Models.Stock> collection = _mongoDbService.GetCollection<Models.Stock>();

            foreach (var orderItem in context.Message.OrderItems)
            {
                bool result = (await collection.FindAsync(s =>
                     s.ProductId == orderItem.ProductId.ToString()
                     && s.Count >= orderItem.Count)).Any();
                stockResult.Add(result);
            }

            if (stockResult.TrueForAll(sr => sr.Equals(true)))
            {
                foreach (var orderItem in context.Message.OrderItems)
                {
                    Models.Stock stock = (await collection.FindAsync(c => c.ProductId == orderItem.ProductId.ToString())).FirstOrDefault();
                    stock.Count -= orderItem.Count;
                    await collection.FindOneAndReplaceAsync(c => c.ProductId == orderItem.ProductId.ToString(), stock);
                }

                var sendEndPoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{RabbitMQSettings.Payment_StockReservedEventQueue}"));

                StockReservedEvent stockReservedEvent = new()
                {
                    BuyerId = context.Message.BuyerId,
                    OrderId = context.Message.OrderId,
                    TotalPrice = context.Message.TotalPrice,
                    OrderItems = context.Message.OrderItems
                };

                await sendEndPoint.Send(stockReservedEvent);
            }
            else
            {
                StockNotReservedEvent stockNotReservedEvent = new StockNotReservedEvent
                {
                    BuyerId = context.Message.BuyerId,
                    OrderId = context.Message.OrderId,
                    Message = "Stock yetersiz"
                };

                await _publishEndPoint.Publish(stockNotReservedEvent);

            }
        }
    }
}

