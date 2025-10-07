using Application.Bookmark.Commands;
using Domain.Entities;
using Domain.Repositories;
using Domain.Shared;
using BookmarkEntity = Domain.Entities.Bookmark;

namespace Application.Bookmark;

public class Creator(
    IUnitOfWork uow,
    ITagRepository tagRepo,
    IBookmarkRepository bookmarkRepo)
{
    public async Task<Result> DoAsync(Create command, CancellationToken cancellationToken = default)
    {
        // begin transaction
        var txBeginResult = await uow.BeginAsync(cancellationToken).ConfigureAwait(false);
        if (txBeginResult.IsFailure)
            return Result.Failure(txBeginResult.Error);

        // check for existing bookmark with same url or title
        List<Task<Result<bool>>> fetcherTasks =
        [
            bookmarkRepo.ExistsByUrlAsync(command.Url, cancellationToken),
            bookmarkRepo.ExistsByTitleAsync(command.Title, cancellationToken)
        ];
        var fetcherResults = await Task.WhenAll(fetcherTasks).ConfigureAwait(false);
        if (Array.Exists(fetcherResults, r => r.IsFailure))
            return Result.Failure(Error.Aggregate(fetcherResults.Select(r => r.Error)));

        if (fetcherResults[0].Value) // URL already exists
            return Result.Failure(Errors.Bookmark.DuplicateUrl);

        if (fetcherResults[1].Value) // Title already exists
            return Result.Failure(Errors.Bookmark.DuplicateTitle);

        List<Guid> tagIds = [];
        if (command.Tags != null && command.Tags.Length > 0) // if there are tags to process
        {
            // fetch existing tags from repository
            var tagFetcherResult =  await tagRepo.GetByNameAsync(command.Tags, cancellationToken).ConfigureAwait(false);
            if (tagFetcherResult.IsFailure)
                return Result.Failure(tagFetcherResult.Error);
            var existingTags = tagFetcherResult.Value;
            tagIds.AddRange(existingTags.Select(p => p.Id));

            // determine which tags need to be created
            var creatableTags = command.Tags.Except(existingTags.Select(t => t.Name)).ToArray();

            // create new tags in repository
            if (creatableTags.Length > 0)
            {
                // create tag entities
                var tagCreatorResults = creatableTags.Select(p => Tag.Create(Guid.NewGuid(), p)).ToArray();
                if (Array.Exists(tagCreatorResults, r => r.IsFailure))
                    return Result.Failure(Error.Aggregate(tagCreatorResults.Select(p => p.Error)));
                var newTags = tagCreatorResults.Select(r => r.Value);
                tagIds.AddRange(newTags.Select(p => p.Id));

                // persist tag entites
                var tagPersisterResult = await tagRepo.CreateAsync(newTags, cancellationToken).ConfigureAwait(false);
                if (tagPersisterResult.IsFailure)
                    return Result.Failure(tagPersisterResult.Error);
            }
        }

        // create bookmark entity
        var bookmarkCreatorResult = BookmarkEntity.Create(
            Guid.NewGuid(),
            command.Url,
            command.Title,
            command.Desc,
            command.IsPublic,
            [.. tagIds]);
        if (bookmarkCreatorResult.IsFailure)
            return Result.Failure(bookmarkCreatorResult.Error);

        // persist bookmark entity
        var bookmarkPersisterResult = await bookmarkRepo.CreateAsync(bookmarkCreatorResult.Value, cancellationToken).ConfigureAwait(false);
        if (bookmarkPersisterResult.IsFailure)
            return Result.Failure(bookmarkPersisterResult.Error);

        // commit transaction
        var txCommitResult = await uow.CommitAsync(cancellationToken).ConfigureAwait(false);
        if (txCommitResult.IsFailure)
            return Result.Failure(txCommitResult.Error);

        return Result.Success();
    }
}
