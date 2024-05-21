using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Infrastructure.Events
{
    internal class EventGridConfiguration
    {
        public const string ConfigurationKey = "EventGrid";
        public required string UserCreatedTopicEndpointUrl { get; set; }
        public required string AccessKey { get; set; }
    }
}
