using Shared.Application.Common.Interfaces;
using Shared.Models;

namespace Shared.Services.Interfaces
{
    public interface IEventPublisher : ITransientService
    {
        Task PublishAsync(IEvent @event);
    }
}
