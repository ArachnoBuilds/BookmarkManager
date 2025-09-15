using Domain.Repositories;
using Domain.Shared;
using BookmarkEntity = Domain.Entities.Bookmark;

namespace Tests.Application;

// Simple in-memory fake implementations to avoid external mocking libraries
internal class FakeBookmarkRepository : IBookmarkRepository
{
    public bool UrlExists { get; set; }
    public bool TitleExists { get; set; }
    public bool CreateCalled { get; private set; }
    public BookmarkEntity? CreatedBookmark { get; private set; }

    // Error simulation fields
    public Error? ExistsByUrlAsyncError { get; set; }
    public Error? ExistsByTitleAsyncError { get; set; }
    public bool FailBookmarkCreate { get; set; }
    public Error? CreateAsyncError { get; set; }

    public Task<Result> CreateAsync(BookmarkEntity bookmark, CancellationToken cancellationToken = default)
    {
        CreateCalled = true;
        CreatedBookmark = bookmark;
        if (CreateAsyncError != null)
            return Task.FromResult(Result.Failure(CreateAsyncError));
        return Task.FromResult(Result.Success());
    }

    public Task<Result<IReadOnlyList<BookmarkEntity>>> GetByIdAsync(Guid[] ids, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Result.Success<IReadOnlyList<BookmarkEntity>>([]));
    }

    public Task<Result<bool>> ExistsByTitleAsync(string title, CancellationToken cancellationToken = default)
    {
        if (ExistsByTitleAsyncError != null)
            return Task.FromResult(Result.Failure<bool>(ExistsByTitleAsyncError));
        return Task.FromResult(Result.Success(TitleExists));
    }

    public Task<Result<bool>> ExistsByUrlAsync(string url, CancellationToken cancellationToken = default)
    {
        if (ExistsByUrlAsyncError != null)
            return Task.FromResult(Result.Failure<bool>(ExistsByUrlAsyncError));
        return Task.FromResult(Result.Success(UrlExists));
    }
}
