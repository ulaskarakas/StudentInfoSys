namespace StudentInfoSys.Data.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        Task<int> SaveChangesAsync();
        Task BeginTransaction();
        Task CommitTransaction();
        Task RollbackTransaction();
    }
}