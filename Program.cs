using FIAP.Messages;
using FIAP.Payments.Consumers;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

Console.WriteLine("PROGRAM NOVA DO PAYMENTS");
Console.WriteLine($"RABBITMQ_HOST={Environment.GetEnvironmentVariable("RABBITMQ_HOST")}");

var rabbitHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "rabbitmq-service";

var orderPlacedQueue = Environment.GetEnvironmentVariable("ORDER_PLACED_QUEUE") ?? "order-placed-queue";

var orderPlacedEntityName = Environment.GetEnvironmentVariable("ORDER_PLACED_ENTITY_NAME") ?? "order-placed-queue";

var paymentProcessedEntityName = Environment.GetEnvironmentVariable("PAYMENT_PROCESSED_ENTITY_NAME") ?? "PaymentProcessedEvent";

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<OrderPlacedEventConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(rabbitHost, "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.Message<OrderPlacedEvent>(x => x.SetEntityName(orderPlacedEntityName));
        cfg.Message<PaymentProcessedEvent>(x => x.SetEntityName(paymentProcessedEntityName));

        cfg.ReceiveEndpoint(orderPlacedQueue, e =>
        {
            e.ConfigureConsumer<OrderPlacedEventConsumer>(context);
        });
    });
});

var app = builder.Build();

app.MapGet("/", () => "FIAP.Payments API running...");
app.MapHealthChecks("/health");

app.Run();