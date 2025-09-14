using Domain.Shared;

namespace Domain;

public static class Errors
{
    public static class Bookmark
    {
        public static Error InvalidUrl =>
            new("Domain.Errros.Bookmark.InvalidUrl", "The URL provided is not valid.");
        public static Error InvalidTitle =>
            new("Domain.Errors.Bookmark.InvalidTitle", "The title provided is not valid.");
    }
}
