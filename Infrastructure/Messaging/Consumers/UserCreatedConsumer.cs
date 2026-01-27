using Infrastructure.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using ConstantsLib.Events;
using Models;
using Business_Layer.Business;
using Business_Layer.JsonService;

public class UserCreatedConsumer : BackgroundService
{
    private readonly RabbitmqConnection RabbitmqConnection;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private IChannel? Channel;
    private const string QueueName = "auth.user-created.ecommerce";

    public UserCreatedConsumer(RabbitmqConnection rabbitmqConnection, IServiceScopeFactory serviceScopeFactory)
    {
        RabbitmqConnection = rabbitmqConnection;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Channel = await RabbitmqConnection.CreateChannel();
        var consumer = new AsyncEventingBasicConsumer(Channel);

        consumer.ReceivedAsync += async (obj, eventArgs) =>
        {
            using var scope = _serviceScopeFactory.CreateScope();

            var userBusiness = scope.ServiceProvider.GetRequiredService<UserBusiness>();

            try
            {
                var @event = Deserializer.DeserializeBytesArray<UserCreatedEvent>(eventArgs.Body.ToArray());

                Console.WriteLine($" [x] Received: {@event}");

                if (@event == null)
                {
                    throw new ArgumentNullException();
                }

                var user = new User
                {
                    id = @event.UserId,
                    name = @event.Name,
                    email = @event.Email
                };

                if (await userBusiness.Add(user))
                {
                    Console.WriteLine($" [x] Message Ack Successfully: {@event}");

                    await Channel.BasicAckAsync(eventArgs.DeliveryTag, false);
                }
                else
                {
                    await Channel.BasicNackAsync(eventArgs.DeliveryTag, false, false);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing message: {ex.Message}");
                await Channel.BasicNackAsync(eventArgs.DeliveryTag, false, false);
            }

        };

        await Channel.BasicConsumeAsync(QueueName, false, consumer, stoppingToken);
    }
}