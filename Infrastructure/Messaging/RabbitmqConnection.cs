using ConstantsLib.Exchanges;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Infrastructure.Messaging
{
    public class RabbitmqConnection : IDisposable
    {
        private IConnectionFactory ConnectionFactory { get; }
        private IConnection Connection { get; set; } = null!;
        private bool IsDisposed { get; set; }

        private readonly SemaphoreSlim ConnectionLock = new SemaphoreSlim(1, 1);

        public RabbitmqConnection(IConnectionFactory connectionFactory)
        {
            ConnectionFactory = connectionFactory;
        }

        private bool IsConnected => Connection != null && Connection.IsOpen && !IsDisposed;

        public async Task<bool> TryConnect()
        {
            if (IsConnected) return true;

            await ConnectionLock.WaitAsync();

            try
            {
                if (IsConnected) return true;

                Connection = await ConnectionFactory.CreateConnectionAsync();

                return IsConnected;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Cannot connect with RabbitMQ : {ex.Message}");
                return false;
            }
            finally
            {
                ConnectionLock.Release();
            }
        }

        public async Task<IChannel> CreateChannel()
        {
            if (!IsConnected && !await TryConnect())
            {
                throw new InvalidOperationException("Cannot Create RabbitMQ Channel");
            }

            return await Connection.CreateChannelAsync();
        }

        public async Task InitializeInfrastructure()
        {
            using var channel = await CreateChannel();

            string UserCreatedQueue = "auth.user-created.ecommerce";
            string ProductCreatedQueue = "catalog.product-created.ecommerce";
            string ProductUpdatedQueue = "catalog.product-updated.ecommerce";

            var authExchange = new AuthExchange();
            var catalogExchange = new CatalogExchange();

            var arguments = new Dictionary<string, object?>
            {
                { "x-queue-type", "quorum" }
            };

            //Declare Exchange
            await channel.ExchangeDeclareAsync(
                exchange: authExchange.Name,
                type: authExchange.Type,
                durable: true
                );

            await channel.ExchangeDeclareAsync(
                exchange: catalogExchange.Name,
                type: catalogExchange.Type,
                durable: true
                );

            //Declare Queue
            await channel.QueueDeclareAsync(
                queue: UserCreatedQueue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: arguments
                );

            await channel.QueueDeclareAsync(
                queue: ProductCreatedQueue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: arguments
                );

            await channel.QueueDeclareAsync(
                queue: ProductUpdatedQueue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: arguments
                );

            //Bind
            await channel.QueueBindAsync(
                queue: ProductCreatedQueue,
                exchange: catalogExchange.Name,
                routingKey: "catalog.product.created"
                );

            await channel.QueueBindAsync(
                queue: ProductUpdatedQueue,
                exchange: catalogExchange.Name,
                routingKey: "catalog.product.updated"
                );

            await channel.QueueBindAsync(
                queue: UserCreatedQueue,
                exchange: authExchange.Name,
                routingKey: "auth.user.created"
                );



        }

        public void Dispose()
        {
            IsDisposed = true;
            Connection?.Dispose();
        }
    }
}
