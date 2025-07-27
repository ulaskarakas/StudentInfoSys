using Microsoft.EntityFrameworkCore.Storage;
using StudentInfoSys.Data.Context;

namespace StudentInfoSys.Data.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly StudentInfoSysDbContext _db;
        private IDbContextTransaction? _transaction;

        public UnitOfWork(StudentInfoSysDbContext db)
        {
            _db = db;
        }

        public async Task BeginTransaction()
        {
            _transaction = await _db.Database.BeginTransactionAsync();
        }

        public async Task CommitTransaction()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
            }
        }

        public async Task RollbackTransaction()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _db.SaveChangesAsync();
        }

        public void Dispose()
        {
            _db.Dispose(); // Cleaning permission granted to Garbage Collector
        }
    }
}