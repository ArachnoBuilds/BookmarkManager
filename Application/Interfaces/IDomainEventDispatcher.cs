using Domain.Primitives;

namespace Application.Interfaces;

public interface IDomainEventDispatcher
{
    Task DoAsync(CancellationToken cancellationToken = default, params IEnumerable<IDomainEvent> events);
}