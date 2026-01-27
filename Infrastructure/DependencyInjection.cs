using Infrastructure.Messaging;
using Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using Infrastructure.Messaging.Consumers;

namespace Infrastructure
{
    public static class DependencyInjection
    {

        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {

            var rabbitmqOptions = configuration.GetSection("RabbitMQ").Get<RabbitmqOptions>();


            services.AddSingleton<RabbitmqConnection>();
            services.AddSingleton<RabbitmqPublisher>();

            services.AddHostedService<RabbitmqInitializer>();

            services.AddHostedService<UserCreatedConsumer>();
            services.AddHostedService<ProductCreatedConsumer>();
            services.AddHostedService<ProductUpdatedConsumer>();


            services.AddSingleton<IConnectionFactory>(sp =>
            {
                return new ConnectionFactory()
                {
                    Uri = new Uri(rabbitmqOptions.Uri)
                };
            });
        }

    }
}
