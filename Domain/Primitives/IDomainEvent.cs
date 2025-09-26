namespace Domain.Primitives;

public interface IDomainEvent
{
    public DateTime OccurredOn { get; init; }
}
