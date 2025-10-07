using Application.Bookmark.Commands;
using Application.Interfaces;
using Domain.Repositories;
using Domain.Shared;

namespace Application.Bookmark;

public class Updater(
    IDomainEventDispatcher eventDispatcher,
    IUnitOfWork uow,
    IBookmarkRepository bookmarkRepo,
    ITagRepository tagRepo)
{
    public async Task<Result> DoAsync(Update command, CancellationToken cancellationToken = default)
    {
        // begin transaction
        var txBeginResult = await uow.BeginAsync(cancellationToken).ConfigureAwait(false);
        if (txBeginResult.IsFailure)
            return Result.Failure(txBeginResult.Error);

        // fetch existing bookmark
        var bookmarkFetcherResult = await bookmarkRepo.GetByIdAsync([command.BookmarkId], cancellationToken).ConfigureAwait(false);
        if (bookmarkFetcherResult.IsFailure)
            return Result.Failure(bookmarkFetcherResult.Error);

        if (bookmarkFetcherResult.Value.Count == 0)
            return Result.Failure(Errors.Bookmark.NotFound);

        var bookmark = bookmarkFetcherResult.Value[0];

        // apply updates in parallel (each update guarded by explicit nullability checks)
        var updateTasks = new List<Task<Result>>();
        if (!string.IsNullOrWhiteSpace(command.Title))
        {
            // capture Title (avoid closure over mutable command)
            var title = command.Title;
            updateTasks.Add(Task.Run(() => bookmark.UpdateTitle(title)));
        }
        if (!string.IsNullOrWhiteSpace(command.Desc))
        {
            // capture Desc (avoid closure over mutable command)
            var desc = command.Desc;
            updateTasks.Add(Task.Run(() => bookmark.UpdateDescription(desc)));
        }
        if (command.IsPublic.HasValue)
        {
            // capture IsPublic (avoid closure over mutable command)
            var isPublic = command.IsPublic.Value;
            updateTasks.Add(Task.Run(() => bookmark.UpdateVisibility(isPublic)));
        }
        if (command.InsertableTags is not null && command.InsertableTags.Length > 0)
        {
            // capture InsertableTags (avoid closure over mutable command)
            var insertableTags = command.InsertableTags;
            updateTasks.Add(Task.Run(async () => 
            {
                var fetcherResult = await tagRepo.GetByNameAsync(insertableTags, cancellationToken).ConfigureAwait(false);
                if (fetcherResult.IsFailure)
                    return Result.Failure(fetcherResult.Error);

                var tags = fetcherResult.Value.Select(p => p.Id).ToArray();
                bookmark.AddTags(tags);

                return Result.Success();
            }));
        }
        if (command.RemovableTags is not null && command.RemovableTags.Length > 0)
        {
            // capture RemovableTags (avoid closure over mutable command)
            var removableTags = command.RemovableTags;
            updateTasks.Add(Task.Run(async () =>
            {
                var fetcherResult = await tagRepo.GetByNameAsync(removableTags, cancellationToken).ConfigureAwait(false);
                if (fetcherResult.IsFailure)
                    return Result.Failure(fetcherResult.Error);

                var tags = fetcherResult.Value.Select(p => p.Id).ToArray();
                bookmark.RemoveTags(tags);

                return Result.Success();
            }));
        }
        if (updateTasks.Count > 0)
        {
            var results = await Task.WhenAll(updateTasks).ConfigureAwait(false);
            if (Array.Exists(results, p => p.IsFailure))
                return Result.Failure(Error.Aggregate(results.Select(p => p.Error)));
        }

        // persist changes
        var updaterResult = await bookmarkRepo.UpdateAsync(bookmark).ConfigureAwait(false);
        if (updaterResult.IsFailure)
            return Result.Failure(updaterResult.Error);

        var events = bookmark.GetDomainEvents();
        await eventDispatcher.DoAsync(cancellationToken, events).ConfigureAwait(false);

        // commit transaction
        var txCommitResult = await uow.CommitAsync(cancellationToken).ConfigureAwait(false);
        if (txCommitResult.IsFailure)
            return Result.Failure(txCommitResult.Error);

        return Result.Success();
    }
}
