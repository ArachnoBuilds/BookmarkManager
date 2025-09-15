using Domain.Entities;
using Domain.Shared;

namespace Domain.Repositories;

public interface ITagRepository
{
    Task<Result<IReadOnlyList<Tag>>> GetByNameAsync(string[] name, CancellationToken cancellationToken = default);
    Task<Result> CreateAsync(IEnumerable<Tag> tags, CancellationToken cancellationToken = default);
}
