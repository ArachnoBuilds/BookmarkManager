using Domain.Events;
using Domain.Primitives;
using Domain.Shared;

namespace Domain.Entities;

public sealed class Bookmark : AggregateRoot, IAuditableEntity
{
    private readonly List<Comment> comments = [];
    private readonly List<Guid> tags = [];

    public string Url { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public bool IsPublic { get; set; }
    public DateTime CreatedOnUtc { get; set; }
    public DateTime? ModifiedOnUtc { get; set; }
    public IReadOnlyList<Comment> Comments => comments;
    public IReadOnlyList<Guid> Tags => tags;

    private Bookmark(Guid id, string url, string title, string? description, bool isPublic, Guid[]? tagIds)
        : base(id)
    {
        Url = url;
        Title = title;
        Description = description;
        IsPublic = isPublic;
        CreatedOnUtc = DateTime.UtcNow;

        if (tagIds != null && tagIds.Length > 0)
            tags.AddRange(tagIds);
    }

    public static Result<Bookmark> Create(Guid id, string url, string title, string? description, bool isPublic, Guid[]? tagIds)
    {
        if (string.IsNullOrWhiteSpace(url) ||
            !Uri.IsWellFormedUriString(url, UriKind.Absolute))
            return Result.Failure<Bookmark>(Errors.Bookmark.InvalidUrl);

        if (string.IsNullOrWhiteSpace(title))
            return Result.Failure<Bookmark>(Errors.Bookmark.InvalidTitle);

        if (tagIds != null && tagIds.Length > 0 && Array.Exists(tagIds, t => t == Guid.Empty))
            return Result.Failure<Bookmark>(Errors.Bookmark.InvalidTag);

        return new Bookmark(id, url, title, description, isPublic, tagIds);
    }

    public Result AddComment(Guid id, string content, Guid author)
    {
        var result = Comment.Create(id, content, author);
        if (result.IsFailure)
            return Result.Failure(result.Error);

        comments.Add(result.Value);
        ModifiedOnUtc = DateTime.UtcNow;
        return Result.Success();
    }

    public Result UpdateTitle(string title)
    {
        if (Title.Equals(title, StringComparison.Ordinal))
            return Result.Failure(Errors.NoChangesDetected);

        if (string.IsNullOrWhiteSpace(title))
            return Result.Failure<Bookmark>(Errors.Bookmark.InvalidTitle);

        Title = title;
        ModifiedOnUtc = DateTime.UtcNow;

        return Result.Success();
    }

    public Result UpdateDescription(string description)
    {
        if (Description?.Equals(description, StringComparison.Ordinal) ?? false)
            return Result.Failure(Errors.NoChangesDetected);

        Description = description;
        ModifiedOnUtc = DateTime.UtcNow;

        return Result.Success();
    }

    public Result UpdateVisibility(bool isPublic)
    {
        if (IsPublic == isPublic)
            return Result.Failure(Errors.NoChangesDetected);

        IsPublic = isPublic;
        ModifiedOnUtc = DateTime.UtcNow;

        return Result.Success();
    }

    public Result AddTags(params IEnumerable<Guid> tagIds)
    {
        if (!tagIds.Any() || tagIds.Contains(Guid.Empty))
            return Result.Failure(Errors.Bookmark.InvalidTag);

        var insertableTags = tagIds.Except(tags);
        if (!insertableTags.Any())
            return Result.Failure(Errors.NoChangesDetected);

        tags.AddRange(insertableTags);
        ModifiedOnUtc = DateTime.UtcNow;
        RaiseDomainEvent(new TagsAddedEvent(Id, [.. insertableTags], ModifiedOnUtc.Value));

        return Result.Success();
    }

    public Result RemoveTags(params IEnumerable<Guid> tagIds)
    {
        if (!tagIds.Any() || tagIds.Contains(Guid.Empty))
            return Result.Failure(Errors.Bookmark.InvalidTag);

        var removableTags = tagIds.Intersect(tags);
        if (!removableTags.Any())
            return Result.Failure(Errors.NoChangesDetected);

        tags.RemoveAll(p => removableTags.Contains(p));
        ModifiedOnUtc = DateTime.UtcNow;
        RaiseDomainEvent(new TagsRemovedEvent(Id, [.. removableTags], ModifiedOnUtc.Value));

        return Result.Success();
    }
}
