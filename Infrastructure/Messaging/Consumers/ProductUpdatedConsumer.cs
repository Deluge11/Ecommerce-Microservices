using Business_Layer.Business;
using Business_Layer.JsonService;
using ConstantsLib.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Messaging.Consumers
{
    public class ProductUpdatedConsumer : BackgroundService
    {
        public ProductUpdatedConsumer(RabbitmqConnection rabbitmqConnection, IServiceProvider serviceProvider)
        {
            RabbitmqConnection = rabbitmqConnection;
            ServiceProvider = serviceProvider;
        }

        public RabbitmqConnection RabbitmqConnection { get; }
        public IServiceProvider ServiceProvider { get; }
        private const string QueueName = "catalog.product-updated.ecommerce";

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var channel = await RabbitmqConnection.CreateChannel();
            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (obj, eventArgs) =>
            {
                await using var scope = ServiceProvider.CreateAsyncScope();
                var productBusiness = scope.ServiceProvider.GetRequiredService<ProductsBusinees>();

                try
                {
                    var @event = Deserializer.DeserializeBytesArray<ProductUpdateEvent>(eventArgs.Body.ToArray());

                    var product = new UpdateProductRequest
                    {
                        id = @event.ProductId,
                        name = @event.Name,
                        price = @event.Price,
                        description = @event.Description,
                        userId = @event.UserId,
                    };

                    Console.WriteLine($" [x] Received: {@event}");

                    if (await productBusiness.Update(product))
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
