using Database.Supports.Contexts;
using Database.Supports.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Database.Supports.Bases
{
    public abstract class BaseRepository<T> : IRepository<T> where T : BaseModel
    {
        public PopAppContext Context { get; }
        internal DbSet<T> DbSet { get; }

        public BaseRepository(PopAppContext context, DbSet<T> dbSet)
        {
            Context = context;
            DbSet = dbSet;
        }

        public virtual async Task<T> Insert(T entity)
        {
            DbSet?.Add(entity);
            await Context.SaveChangesAsync();
            var id = entity.Id;
            IQueryable<T> query = DbSet;
            return await query?.FirstOrDefaultAsync(x => x.Id == id);
        }

        public virtual async Task Delete(int id)
        {
            IQueryable<T> query = DbSet;
            var entityToDelete = await query?.FirstOrDefaultAsync(x => x.Id == id);
            DbSet?.Remove(entityToDelete);
            await Context.SaveChangesAsync();
        }

        public virtual async Task<T> Get(int id)
        {
            IQueryable<T> query = DbSet;
            return await query?.FirstOrDefaultAsync(x => x.Id == id);
        }

        public virtual async Task<IEnumerable<T>> GetAll(object parametersObject = null)
        {
            IQueryable<T> query = DbSet;
            return await query?.ToListAsync();
        }

        public virtual async Task<T> Update(int id, T entity)
        {
            entity.Id = id;
            DbSet?.Update(entity);
            await Context.SaveChangesAsync();
            IQueryable<T> query = DbSet;
            return await query?.FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
