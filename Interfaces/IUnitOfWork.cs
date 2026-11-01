namespace AsyncBatchProcessor.Interfaces
{
    public interface IUnitOfWork
    {
        Task BeginTransactionAsync(CancellationToken ct);

        Task<int> SaveChangesAsync(CancellationToken ct);

        Task CommitAsync(CancellationToken ct);

        Task RollbackAsync(CancellationToken ct);
    }
}
