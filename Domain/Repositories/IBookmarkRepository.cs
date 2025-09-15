using Domain.Entities;
using Domain.Shared;

namespace Domain.Repositories;

public interface IBookmarkRepository
{
    Task<Result<IReadOnlyList<Bookmark>>> GetByIdAsync(Guid[] ids, CancellationToken cancellationToken = default);
    Task<Result<bool>> ExistsByTitleAsync(string title, CancellationToken cancellationToken = default);
    Task<Result<bool>> ExistsByUrlAsync(string url, CancellationToken cancellationToken = default);
    Task<Result> CreateAsync(Bookmark bookmark, CancellationToken cancellationToken = default);
}
