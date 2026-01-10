using Infrastructure.Messaging;
using Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace Infrastructure
{
    public static class DependencyInjection
    {

        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {

            var rabbitmqOptions = configuration.GetSection("RabbitMQ").Get<RabbitmqOptions>();

            services.AddSingleton<RabbitmqConnection>();
            services.AddHostedService<RabbitmqInitializer>();
            services.AddSingleton<RabbitmqPublisher>();
            services.AddHostedService<RabbitmqConsumer>();
            services.AddSingleton<IConnectionFactory>(sp =>
            {
                return new ConnectionFactory()
                {
                    Uri = new Uri(rabbitmqOptions.Uri)
                };
            });

            return services;
        }

    }
}
