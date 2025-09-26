using Domain.Primitives;
using Domain.Shared;

namespace Application.Interfaces;

public interface IDomainEventHandler<TEvent> where TEvent : IDomainEvent
{
    Task<Result> DoAsync(TEvent domainEvent, CancellationToken cancellationToken = default);
}
