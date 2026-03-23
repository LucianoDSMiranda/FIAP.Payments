using CloudGames.Notifications.Application.IntegrationEvents.Purchases;
using FIAP.Messages;
using FIAP.Messages.Enums;
using MassTransit;

namespace FIAP.Payments.Consumers;

public class OrderPlacedEventConsumer : IConsumer<OrderPlacedEvent>
{
    private readonly IPublishEndpoint _publishEndpoint;

    public OrderPlacedEventConsumer(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public async Task Consume(ConsumeContext<OrderPlacedEvent> context)
    {
        var message = context.Message;

        var isApproved = Random.Shared.Next(2) == 0;
        var status = isApproved ? PaymentStatus.Approved : PaymentStatus.Rejected;
      

        await _publishEndpoint.Publish(new PaymentProcessedEvent
        {
            UserId = message.UserId,
            GameId = message.GameId,
            Price = message.Price,
            Status = status,
            Email = message.Email
        });

        Console.WriteLine($"Payment processed for User {message.UserId}, Game {message.GameId}, Status: {status}");
    }
}