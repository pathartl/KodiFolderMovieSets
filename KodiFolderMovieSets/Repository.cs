using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace KodiFolderMovieSets
{
    public class Repository<TObject> where TObject : class
    {
        protected DatabaseContext Context = null;
        private bool shareContext = true;

        public Repository(DatabaseContext _context)
        {
            Context = _context;
        }

        public Repository()
        {
        }

        protected DbSet<TObject> DbSet
        {
            get
            {
                return Context.Set<TObject>();
            }
        }

        public void OpenConnection()
        {
            if (Context != null)
            {
                Context.Database.OpenConnection();
            }
        }

        public void CloseConnection()
        {
            if (Context != null)
            {
                Context.Database.CloseConnection();
            }
        }

        public void Dispose()
        {
            if (!shareContext && (Context != null))
            {
                Context.Dispose();
                Context = null;
            }
        }

        public virtual IQueryable<TObject> All(bool track = true, string[] children = null)
        {
            var objectSet = DbSet.AsQueryable();

            if (children != null)
            {
                foreach (string child in children)
                {
                    objectSet = objectSet.Include(child);
                }
            }

            if (track)
            {
                return objectSet;
            }

            return objectSet.AsNoTracking();
        }

        public virtual IQueryable<TObject> Get(Expression<Func<TObject, bool>> predicate, bool track = true, string[] children = null)
        {
            var objectSet = DbSet.AsQueryable();

            if (children != null)
            {
                foreach (string child in children)
                {
                    objectSet = objectSet.Include(child);
                }
            }

            if (track)
            {
                return objectSet.Where(predicate).AsQueryable<TObject>();
            }

            return objectSet.Where(predicate).AsNoTracking().AsQueryable<TObject>();
        }

        public virtual IQueryable<TObject> Get<Key>(Expression<Func<TObject, bool>> filter, out int total, int index = 0, int size = 50, bool track = true, string[] children = null)
        {
            var _resetSet = DbSet.AsQueryable();

            if (children != null)
            {
                foreach (string child in children)
                {
                    _resetSet = _resetSet.Include(child);
                }
            }

            int skipCount = index * size;
            _resetSet = filter != null ? DbSet.Where(filter).AsQueryable() : DbSet.AsQueryable();
            _resetSet = skipCount == 0 ? _resetSet.Take(size) : _resetSet.Skip(skipCount).Take(size);
            total = _resetSet.Count();

            if (track)
            {
                return _resetSet.AsQueryable();
            }

            return _resetSet.AsNoTracking().AsQueryable();
        }

        public virtual TObject FirstOrDefault(Expression<Func<TObject, bool>> predicate, bool track = true, string[] children = null)
        {
            var objectSet = DbSet.AsQueryable();

            if (children != null)
            {
                foreach (string child in children)
                {
                    objectSet = objectSet.Include(child);
                }
            }

            if (track)
            {
                return objectSet.Where(predicate).FirstOrDefault<TObject>(predicate);
            }

            return objectSet.Where(predicate).AsNoTracking().FirstOrDefault<TObject>(predicate);
        }

        public bool Contains(Expression<Func<TObject, bool>> predicate)
        {
            return DbSet.Count(predicate) > 0;
        }

        public virtual TObject Find(params object[] keys)
        {
            return DbSet.Find(keys);
        }

        public virtual TObject Find(Expression<Func<TObject, bool>> predicate)
        {
            return DbSet.FirstOrDefault(predicate);
        }

        public virtual int Count
        {
            get
            {
                return DbSet.Count();
            }
        }
    }
}
