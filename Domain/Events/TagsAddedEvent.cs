namespace Domain.Events;

public record TagsAddedEvent(Guid BookmarkId, Guid[] TagIds, DateTime OccuredOn) : DomainEvent(OccuredOn);