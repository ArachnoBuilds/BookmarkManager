using Domain.Primitives;
using Domain.Shared;

namespace Domain.Entities;

public class Tag : AggregateRoot, IAuditableEntity
{
    public string Name { get; set; }
    public DateTime CreatedOnUtc { get; set; }
    public DateTime? ModifiedOnUtc { get; set; }

    private Tag(Guid id, string name) 
        : base(id)
    {
        Name = name;
        CreatedOnUtc = DateTime.UtcNow;
    }

    public static Result<Tag> Create(Guid id, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<Tag>(Errors.Tag.InvalidName);
        
        return new Tag(id, name);
    }
}
