using Domain.Shared;

namespace Domain.Repositories;

public interface IUnitOfWork
{
    Task<Result> BeginAsync(CancellationToken cancellationToken = default);
    Task<Result> CommitAsync(CancellationToken cancellationToken = default);
    Task<Result> RollbackAsync(CancellationToken cancellationToken = default);
}
