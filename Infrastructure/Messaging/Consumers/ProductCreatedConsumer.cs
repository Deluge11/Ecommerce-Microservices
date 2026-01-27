using Microsoft.Extensions.Hosting;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using ConstantsLib.Events;
using Microsoft.Extensions.DependencyInjection;
using Business_Layer.Business;
using Models;
using System;
using RabbitMQ.Client;
using Business_Layer.JsonService;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Messaging.Consumers
{
    public class ProductCreatedConsumer : BackgroundService
    {
        public RabbitmqConnection RabbitmqConnection { get; }
        public IServiceProvider ServiceProvider { get; }

        private const string QueueName = "catalog.product-created.ecommerce";

        public ProductCreatedConsumer(RabbitmqConnection rabbitmqConnection, IServiceProvider serviceProvider)
        {
            RabbitmqConnection = rabbitmqConnection;
            ServiceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var channel = await RabbitmqConnection.CreateChannel();
            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (obj, eventArgs) =>
            {
                await using var scope = ServiceProvider.CreateAsyncScope();
                var productBusinees =  scope.ServiceProvider.GetRequiredService<ProductsBusinees>();

                try
                {
                    var @event = Deserializer.DeserializeBytesArray<ProductCreatedEvent>(eventArgs.Body.ToArray());

                    if (@event == null)
                    {
                        throw new ArgumentNullException();
                    }

                    var product = new InsertProductRequest
                    {
                        id = @event.ProductId,
                        name = @event.Name,
                        createdDate = @event.CreatedAt,
                        description = @event.Description,
                        price = @event.Price,
                        userId = @event.UserId
                    };

                    Console.WriteLine($" [x] Received: {@event}");


                    if (await productBusinees.Add(product))
                    {
                        Console.WriteLine($" [x] Handled: {@event}");
                        await channel.BasicAckAsync(eventArgs.DeliveryTag, false);
                    }
                    else
                    {
                        Console.WriteLine($" [x] Cant Handle: {@event}");
                        await channel.BasicNackAsync(eventArgs.DeliveryTag, false, false);
                    }
                }
                catch (Exception)
                {
                    await channel.BasicNackAsync(eventArgs.DeliveryTag, false, false);
                }
            };
            await channel.BasicConsumeAsync(QueueName, false, consumer);
            await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 10, global: false);
        }
    }
}
