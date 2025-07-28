using MediatR;
using Shared.Models;

namespace Shared.Services
{
    public class EventNotification<TEvent> : INotification
    where TEvent : IEvent
    {
        public EventNotification(TEvent @event) => Event = @event;

        public TEvent Event { get; }
    }
}
