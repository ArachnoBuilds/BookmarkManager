using Domain.Shared;

namespace Application.Interfaces.Repositories;

public interface IBookmarkTagRepository
{
    Task<Result> AddTagsAsync(Guid bookmarkId, Guid[] tagIds, CancellationToken cancellationToken = default);
    Task<Result> RemoveTagsAsync(Guid bookmarkId, Guid[] tagIds, CancellationToken cancellationToken = default);
}
