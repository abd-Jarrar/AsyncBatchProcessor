using AsyncBatchProcessor.Data;
using AsyncBatchProcessor.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace AsyncBatchProcessor.Services
{
    public class UnitOfWork(AppDbContext context) : IUnitOfWork
    {
        private IDbContextTransaction? _transaction;

        public async Task BeginTransactionAsync(CancellationToken ct)
        {
            _transaction = await context.Database
                .BeginTransactionAsync(ct);
        }

        public Task<int> SaveChangesAsync(CancellationToken ct)
        {
            return context.SaveChangesAsync(ct);
        }

        public async Task CommitAsync(CancellationToken ct)
        {
            if (_transaction is null)
                throw new InvalidOperationException(
                    "No active transaction.");

            await _transaction.CommitAsync(ct);
            await _transaction.DisposeAsync();
            _transaction = null;
        }

        public async Task RollbackAsync(CancellationToken ct)
        {
            if (_transaction is null)
                return;

            try
            {
                await _transaction.RollbackAsync(ct);
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }
}
