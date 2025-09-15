using Domain.Shared;

namespace Application;

internal static class Errors
{
    internal static class Bookmark
    {
        public static readonly Error DuplicateUrl = new(
            "Application.Errors.Bookmark.DuplicateUrl",
            "A bookmark with the same URL already exists.");
        public static readonly Error DuplicateTitle = new(
            "Application.Errors.Bookmark.DuplicateTitle",
            "A bookmark with the same title already exists.");
    }
}
