using Azure;
using CommunityToolkit.Diagnostics;
using Identity.Domain.Interfaces;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Infrastructure.Events
{
    internal static class EventGridServiceExtension
    {
        internal static IServiceCollection AddEventGrid(this IServiceCollection services, IConfiguration configuration)
        {
            var section = configuration.GetSection(EventGridConfiguration.ConfigurationKey);
            services.Configure<EventGridConfiguration>(section);

            var topicEndpoint = section.GetValue<string>("UserCreatedTopicEndpointUrl");
            var accessKey = section.GetValue<string>("AccessKey");

            Guard.IsNotNullOrWhiteSpace(topicEndpoint);
            Guard.IsNotNullOrWhiteSpace(accessKey);

            services.AddAzureClients(clients =>
            {
                clients.AddEventGridPublisherClient(new Uri(topicEndpoint), new AzureKeyCredential(accessKey));
            });

            services.AddTransient<IEventConnector, EventGridConnector>();

            return services;
        }
    }
}
