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
        public static Error InvalidTag =>
            new("Domain.Errors.Bookmark.InvalidTag", "One or more tags provided are not valid.");
    }

    public static class Comment
    {
        public static Error InvalidContent =>
            new("Domain.Errors.Comment.InvalidContent", "The content provided is not valid.");
        public static Error InvalidAuthor =>
            new("Domain.Errors.Comment.InvalidAuthor", "The author provided is not valid.");
    }

    public static class Tag
    {
        public static Error InvalidName =>
            new("Domain.Errors.Tag.InvalidName", "The name provided is not valid.");
    }
}
