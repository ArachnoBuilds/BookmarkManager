namespace Application.Bookmark.Commands;

public record Update(Guid BookmarkId)
{
    public string? Title { get; set; }
    public string? Desc { get; set; }
    public bool? IsPublic { get; set; }
    public string[]? InsertableTags { get; set; }
    public string[]? RemovableTags { get; set; }
}
