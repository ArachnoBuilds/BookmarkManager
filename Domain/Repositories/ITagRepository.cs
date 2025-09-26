using Domain.Entities;
using Domain.Shared;

namespace Domain.Repositories;

public interface ITagRepository
{
    Task<Result<IReadOnlyList<Tag>>> GetByNameAsync(string[] names, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<KeyValuePair<Guid, bool>>>> ExistsByIdAsync(Guid[] ids, CancellationToken cancellationToken = default);
    Task<Result> CreateAsync(IEnumerable<Tag> tags, CancellationToken cancellationToken = default);
}
