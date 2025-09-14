namespace Domain.Primitives;

public abstract class AggregateRoot : Entity
{
    private readonly List<IDomainEvent> _events = [];

    protected AggregateRoot(Guid id)
        : base(id)
    {
    }

    protected AggregateRoot()
    {
    }

    public IReadOnlyCollection<IDomainEvent> GetDomainEvents() => [.. _events];

    public void ClearDomainEvents() => _events.Clear();

    protected void RaiseDomainEvent(IDomainEvent domainEvent) =>
        _events.Add(domainEvent);
}
