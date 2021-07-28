using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sgm.OC.Framework;

namespace Sgm.OC.Repository
{

    public class QueryRepository<T> where T : class
    {

        public readonly SgmOcContext _context;

        public QueryRepository(SgmOcContext context)
        {
            _context = context;
        }

        public DbSet<T> DbSet
        {
            get
            {
                return _context.Set<T>();
            }
        }


    }


    public class GenericRepository<TEntity> where TEntity : EntityBase
    {

        private readonly SgmOcContext  _context;

        public GenericRepository(SgmOcContext context)
        {
            _context = context;
        }

        public DbSet<TEntity> DbSet
        {
            get
            {
                return _context.Set<TEntity>();
            }
        }

        public TEntity GetById(int id)
        {
            return _context.Set<TEntity>().FirstOrDefault(e => e.Id == id);
        }

        public IQueryable<TEntity> GetQueryable()
        {
            return _context.Set<TEntity>().AsQueryable();
        }

        public void Update(TEntity entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(TEntity entity)
        {
            _context.Entry(entity).State = EntityState.Deleted;
        }

        public void Create(TEntity entity)
        {
            _context.Entry(entity).State = EntityState.Added;
        }

        public EntityState GetEntityState(TEntity entity)
        {
            return _context.Entry(entity).State;
        }

    }
}
