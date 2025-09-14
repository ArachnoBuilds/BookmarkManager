using Domain.Primitives;
using Domain.Shared;

namespace Domain.Entities;

public sealed class Bookmark : AggregateRoot, IAuditableEntity
{
    public string Url { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public bool IsPublic { get; set; }
    public DateTime CreatedOnUtc { get; set; }
    public DateTime? ModifiedOnUtc { get; set; }

    private Bookmark(Guid id, string url, string title, string? description, bool isPublic)
        : base(id)
    {
        Url = url;
        Title = title;
        Description = description;
        IsPublic = isPublic;
        CreatedOnUtc = DateTime.UtcNow;
    }

    public static Result<Bookmark> Create(Guid id, string url, string title, string? description, bool isPublic)
    {
        if (string.IsNullOrWhiteSpace(url) ||
            !Uri.IsWellFormedUriString(url, UriKind.Absolute))
            return Result.Failure<Bookmark>(Errors.Bookmark.InvalidUrl);

        if (string.IsNullOrWhiteSpace(title))
            return Result.Failure<Bookmark>(Errors.Bookmark.InvalidTitle);

        return new Bookmark(id, url, title, description, isPublic);
    }
}
