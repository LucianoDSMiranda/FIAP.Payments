using FIAP.Messages;
using FIAP.Payments.Consumers;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<OrderPlacedEventConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", h => { h.Username("guest");h.Password("guest");});
        cfg.Message<OrderPlacedEvent>(x => x.SetEntityName("OrderPlacedEvent"));
        cfg.Message<PaymentProcessedEvent>(x => x.SetEntityName("PaymentProcessedEvent"));
        cfg.ReceiveEndpoint("order-placed-queue", e => { e.ConfigureConsumer<OrderPlacedEventConsumer>(context); });
    });
});
var app = builder.Build();

app.MapGet("/", () => "FIAP.Payments API running...");
app.MapHealthChecks("/health");

app.Run();