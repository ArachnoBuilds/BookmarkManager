using Application.Interfaces;
using Application.Interfaces.Repositories;
using Domain.Events;
using Domain.Repositories;
using Domain.Shared;

namespace Application.EventHandlers;

internal class TagsAddedEventHandler(
    IBookmarkRepository bookmarkRepository,
    ITagRepository tagRepository,
    IBookmarkTagRepository bookmarkTagRepository) 
    : IDomainEventHandler<TagsAddedEvent>
{
    public async Task<Result> DoAsync(TagsAddedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        var (bookmarkId, tagIds, _) = domainEvent;

        // check if bookmark exists
        var bookmarkReaderResult = await bookmarkRepository.ExistsByIdAsync(bookmarkId, cancellationToken);
        if (bookmarkReaderResult.IsFailure)
            return bookmarkReaderResult;
        if (!bookmarkReaderResult.Value)
            return Result.Failure(Errors.Bookmark.NotFound);

        // check if tags exist
        var readerResult = await tagRepository.ExistsByIdAsync(tagIds, cancellationToken);
        if (readerResult.IsFailure)
            return readerResult;
        if (readerResult.Value.Any(p => !p.Value))
            return Result.Failure(Errors.Tag.NotFound);

        // persist tags
        var updaterResult = await bookmarkTagRepository.AddTagsAsync(bookmarkId, tagIds, cancellationToken);
        if (updaterResult.IsFailure)
            return updaterResult;

        return Result.Success();
    }
}