using Domain.Repositories;
using Domain.Shared;
using Domain.Entities;

namespace Tests.Application;

internal class FakeTagRepository : ITagRepository
{
    public List<Tag> TagsToReturn { get; set; } = new();
    public bool CreateCalled { get; private set; }

    // Error simulation fields
    public Error? GetByNameAsyncError { get; set; }
    public string? FailTagCreateFor { get; set; }
    public Error? CreateAsyncError { get; set; }

    public Task<Result> CreateAsync(IEnumerable<Tag> tags, CancellationToken cancellationToken = default)
    {
        CreateCalled = true;
        if (CreateAsyncError != null)
            return Task.FromResult(Result.Failure(CreateAsyncError));
        return Task.FromResult(Result.Success());
    }

    public Task<Result<IReadOnlyList<Tag>>> GetByNameAsync(string[] name, CancellationToken cancellationToken = default)
    {
        if (GetByNameAsyncError != null)
            return Task.FromResult(Result.Failure<IReadOnlyList<Tag>>(GetByNameAsyncError));
        var found = TagsToReturn.Where(t => name.Contains(t.Name)).ToList();
        return Task.FromResult(Result.Success<IReadOnlyList<Tag>>(found));
    }

    public static Result<Tag> Create(Guid id, string name)
    {
        if (!string.IsNullOrWhiteSpace(name) && name == "badtag")
            return Result.Failure<Tag>(new Error("Tag.InvalidName", "Invalid tag name"));
        return Domain.Entities.Tag.Create(id, name);
    }
}
