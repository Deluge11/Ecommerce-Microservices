using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Messaging
{
    public class RabbitmqInitializer : IHostedService
    {
        private RabbitmqConnection RabbitmqConnection { get; }

        public RabbitmqInitializer(RabbitmqConnection rabbitmqConnection)
        {
            RabbitmqConnection = rabbitmqConnection;
        }


        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await RabbitmqConnection.InitializeInfrastructure();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
