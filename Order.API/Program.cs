using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.API.Consumer;
using Order.API.Models;
using Order.API.Models.Contexts;
using Order.API.ViewModels;
using Shared;
using Shared.Events;
using Shared.Messages;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(configurator =>
{
    configurator.AddConsumer<PaymentCompletedEventConsumer>();
    configurator.AddConsumer<PaymentFailedEventConsumer>();
    configurator.UsingRabbitMq((context, configurator) =>
    {
        configurator.Host(builder.Configuration["RabbitMQ"]);
        configurator.ReceiveEndpoint(RabbitMQSettings.Order_PaymentCompletedEventQueue,
            e => e.ConfigureConsumer<PaymentCompletedEventConsumer>(context));
        configurator.ReceiveEndpoint(RabbitMQSettings.Order_PaymentFailedEventQueue,
           e => e.ConfigureConsumer<PaymentFailedEventConsumer>(context));
    });
});

builder.Services.AddDbContext<OrderApiContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SQLServer")));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapPost("/create-order", async (CreateOrderVM model, OrderApiContext context, IPublishEndpoint publishEndpoint) =>
{
    Order.API.Models.Order order = new Order.API.Models.Order()
    {
        BuyerId = Guid.TryParse(model.BuyerId, out Guid _buyerId) ? _buyerId : Guid.NewGuid(),
        OrderItems = model.OrderItems.Select(c => new OrderItem
        {
            Count = c.Count,
            Price = c.Price,
            ProductId = Guid.TryParse(c.ProductId, out Guid ProductId) ? ProductId : Guid.NewGuid()
        }).ToList(),
        OrderStatus = Order.API.Enums.OrderStatus.Suspend,
        CreateDate = DateTime.UtcNow,
        TotalPrice = model.OrderItems.Sum(c => c.Price * c.Count)
    };

    await context.Orders.AddAsync(order);
    await context.SaveChangesAsync();

    OrderCreatedEvent orderCreatedEvent = new()
    {
        BuyerId = order.BuyerId,
        OrderId = order.Id,
        TotalPrice = order.TotalPrice,
        OrderItems = order.OrderItems.Select(c => new OrderItemMessage
        {
            Count = c.Count,
            Price = c.Price,
            ProductId = c.ProductId
        }).ToList()
    };

    await publishEndpoint.Publish(orderCreatedEvent);
});

app.Run();

