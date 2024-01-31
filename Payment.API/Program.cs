using MassTransit;
using Payment.API.Consumer;
using Shared;
using Shared.Events;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit(configurator =>
{
    configurator.AddConsumer<StockReservedEventConsumer>();
    configurator.UsingRabbitMq((context, configurator) =>
    {
        configurator.Host(builder.Configuration["RabbitMQ"]);
        configurator.ReceiveEndpoint(RabbitMQSettings.Payment_StockReservedEventQueue,
            e => e.ConfigureConsumer<StockReservedEventConsumer>(context));
    });
});

var app = builder.Build();


app.Run();

