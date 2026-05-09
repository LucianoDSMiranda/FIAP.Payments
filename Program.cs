using FIAP.Messages;
using FIAP.Payments.Consumers;
using FIAP.Payments.Logging;
using MassTransit;
using Prometheus;
using Serilog;

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseCloudGamesLogging("payments-api");

    var rabbitHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "rabbitmq-service";
    var orderPlacedQueue = Environment.GetEnvironmentVariable("ORDER_PLACED_QUEUE") ?? "order-placed-queue";
    var orderPlacedEntityName = Environment.GetEnvironmentVariable("ORDER_PLACED_ENTITY_NAME") ?? "order-placed-queue";
    var paymentProcessedEntityName = Environment.GetEnvironmentVariable("PAYMENT_PROCESSED_ENTITY_NAME") ?? "PaymentProcessedEvent";

    Log.Information("Starting FIAP.Payments API...");
    Log.Information("RabbitMQ Host: {RabbitHost}", rabbitHost);

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

    app.UseHttpMetrics();

    app.MapGet("/", () => "FIAP.Payments API running...");
    app.MapHealthChecks("/health");
    app.MapMetrics();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application stopped because of exception");
    throw;
}
finally
{
    Log.CloseAndFlush();
}
