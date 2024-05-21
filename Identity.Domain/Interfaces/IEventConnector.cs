using Identity.Domain.Models;

namespace Identity.Domain.Interfaces
{
    public interface IEventConnector
    {
        Task SendUserCreatedEvent(User user, CancellationToken cancellationToken);
    }
}
