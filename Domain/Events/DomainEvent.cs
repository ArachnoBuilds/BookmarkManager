using Domain.Primitives;

namespace Domain.Events;

public abstract record DomainEvent(DateTime OccurredOn) : IDomainEvent;