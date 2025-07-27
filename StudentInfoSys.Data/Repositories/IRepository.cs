using System.Linq.Expressions;

namespace StudentInfoSys.Data.Repositories
{
    public interface IRepository<TEntity> where TEntity : class
    {
        // Gets an entity by its unique identifier
        Task<TEntity?> GetByIdAsync(int id);

        //Retrieves all entities from the database
        Task<List<TEntity>> GetAllAsync();

        // Retrieves entities that match a given filter expression
        Task<List<TEntity>> GetWhereAsync(Expression<Func<TEntity, bool>> predicate);

        // Retrieves a single entity that matches a given filter expression
        Task<TEntity?> GetSingleAsync(Expression<Func<TEntity, bool>> predicate);

        // Adds a new entity to the database asynchronously
        Task AddAsync(TEntity entity);

        // Updates an existing entity in the database
        void Update(TEntity entity);

        // Marks an entity as deleted (soft delete) or removes it from the database
        void Delete(TEntity entity);
    }
}