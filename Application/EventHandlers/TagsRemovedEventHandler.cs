using Application.Interfaces;
using Application.Interfaces.Repositories;
using Domain.Events;
using Domain.Repositories;
using Domain.Shared;

namespace Application.EventHandlers;

internal class TagsRemovedEventHandler(
    IBookmarkRepository bookmarkRepository,
    IBookmarkTagRepository bookmarkTagRepository) 
    : IDomainEventHandler<TagsRemovedEvent>
{
    public async Task<Result> DoAsync(TagsRemovedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        var (bookmarkId, tagIds, _) = domainEvent;

        // check if bookmark exists
        var bookmarkReaderResult = await bookmarkRepository.ExistsByIdAsync(bookmarkId, cancellationToken);
        if (bookmarkReaderResult.IsFailure)
            return bookmarkReaderResult;
        if (!bookmarkReaderResult.Value)
            return Result.Failure(Errors.Bookmark.NotFound);

        // persist tags
        var updaterResult = await bookmarkTagRepository.RemoveTagsAsync(bookmarkId, tagIds, cancellationToken);
        if (updaterResult.IsFailure)
            return updaterResult;

        return Result.Success();
    }
}
