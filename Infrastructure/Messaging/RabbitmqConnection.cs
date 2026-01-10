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

            string QueueName = "TestQ";

            var arguments = new Dictionary<string, object?>
            {
                { "x-queue-type", "quorum" }
            };


            await channel.ExchangeDeclareAsync(
                exchange: "auth.events",
                type: ExchangeType.Topic,
                durable: true
                );


            await channel.QueueDeclareAsync(
                queue: QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: arguments
                );


            await channel.QueueBindAsync(
                queue: QueueName,
                exchange: "auth.events",
                routingKey: "auth.user.created"
                );

            await channel.BasicQosAsync(0, 1, false);

        }

        public void Dispose()
        {
            IsDisposed = true;
            Connection?.Dispose();
        }
    }
}
