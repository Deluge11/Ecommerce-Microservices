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

public class RabbitmqConsumer : BackgroundService
{
    private readonly RabbitmqConnection RabbitmqConnection;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private IChannel? Channel;
    private const string QueueName = "TestQ";

    public RabbitmqConsumer(RabbitmqConnection rabbitmqConnection, IServiceScopeFactory serviceScopeFactory)
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
                var body = eventArgs.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var userCreatedMessage = JsonSerializer.Deserialize<UserCreatedEvent>(message);

                Console.WriteLine($" [x] Received: {message}");

                if (userCreatedMessage == null)
                {
                    throw new ArgumentNullException();
                }

                var user = new User
                {
                    id = userCreatedMessage.UserId,
                    name = userCreatedMessage.Name,
                    email = userCreatedMessage.Email
                };

                if (await userBusiness.Add(user))
                {
                    Console.WriteLine($" [x] Message Ack Successfully: {message}");

                    await Channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing message: {ex.Message}");
                await Channel.BasicNackAsync(eventArgs.DeliveryTag, multiple: false, false);
            }

        };

        await Channel.BasicConsumeAsync(QueueName, false, consumer, stoppingToken);
    }
}