namespace Domain.Events;

public record TagsRemovedEvent(Guid BookmarkId, Guid[] TagIds, DateTime OccuredOn) : DomainEvent(OccuredOn);
