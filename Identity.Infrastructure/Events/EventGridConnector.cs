using Azure.Messaging.EventGrid;
using Identity.Domain.Interfaces;
using Identity.Domain.Models;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;

namespace Identity.Infrastructure.Events
{
    internal class EventGridConnector(EventGridPublisherClient client) : IEventConnector
    {
        private const string UserCreatedEventType = "fb/identity/user/created";
        private const string DataVersion = "1.0";

        async Task IEventConnector.SendUserCreatedEvent(User user, CancellationToken cancellationToken)
        {
            try
            {
                var createdEvent = CreateUserCreatedEvent(user);
                await client.SendEventAsync(createdEvent, cancellationToken);
            }
            catch (Exception e)
            {
                Log.Error(e, "Something went wrong publishing user event. Event type {eventType}, User id {userId}", UserCreatedEventType, user.Id);
                throw;
            }
        }

        private static EventGridEvent CreateUserCreatedEvent(User user)
        {
            return new EventGridEvent(user.Id, UserCreatedEventType, DataVersion, new
            {
                user.Id,
                user.Email,
                user.FirstName,
                user.LastName
            });
        }
    }
}
