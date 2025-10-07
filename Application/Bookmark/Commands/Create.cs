namespace Application.Bookmark.Commands;

public record Create(string Url, string Title, bool IsPublic, string? Desc = null, string[]? Tags = null);