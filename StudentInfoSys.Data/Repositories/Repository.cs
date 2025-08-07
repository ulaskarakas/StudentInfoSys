using Microsoft.EntityFrameworkCore;
using StudentInfoSys.Data.Context;
using StudentInfoSys.Data.Entities;
using System.Linq.Expressions;

namespace StudentInfoSys.Data.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
    {
        protected readonly StudentInfoSysDbContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        public Repository(StudentInfoSysDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }

        // Adds a new entity to the database asynchronously
        public async Task AddAsync(TEntity entity)
        {
            entity.CreatedDate = DateTime.UtcNow;
            await _dbSet.AddAsync(entity);
        }

        //Retrieves all entities from the database
        public async Task<List<TEntity>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        // Gets an entity by its unique identifier
        public async Task<TEntity?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        // Retrieves a single entity that matches a given filter expression
        public async Task<TEntity?> GetSingleAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }

        // Retrieves entities that match a given filter expression
        public async Task<List<TEntity>> GetWhereAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        // Returns the number of entities that match the given filter expression
        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.CountAsync(predicate);
        }

        // Updates an existing entity in the database
        public void Update(TEntity entity)
        {
            entity.ModifiedDate = DateTime.UtcNow;
            _dbSet.Update(entity);
        }

        // Marks an entity as deleted (soft delete) or removes it from the database
        public void Delete(TEntity entity)
        {
            entity.ModifiedDate = DateTime.UtcNow;
            entity.IsDeleted = true;  // soft delete
            _dbSet.Update(entity);
        }
    }
}