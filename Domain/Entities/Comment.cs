using Domain.Primitives;
using Domain.Shared;

namespace Domain.Entities;

public class Comment: Entity, IAuditableEntity
{
    public string Content { get; set; }
    public Guid Author { get; set; }
    public DateTime CreatedOnUtc { get; set; }
    public DateTime? ModifiedOnUtc { get; set; }

    private Comment(Guid id, string content, Guid author): base(id)
    {
        Content = content;
        Author = author;
        CreatedOnUtc = DateTime.UtcNow;
    }

    public static Result<Comment> Create(Guid id, string content, Guid author)
    {
        if (string.IsNullOrWhiteSpace(content))
            return Result.Failure<Comment>(Errors.Comment.InvalidContent);
        
        if (author == Guid.Empty)
            return Result.Failure<Comment>(Errors.Comment.InvalidAuthor);
        
        return new Comment(id, content, author);
    }
}
