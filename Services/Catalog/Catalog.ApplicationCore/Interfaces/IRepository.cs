using Catalog.ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.ApplicationCore.Interfaces
{
    public interface IRepository<TKey, TEntity> where TEntity : class, IEntity<TKey>
    {
        public IQueryable<TEntity> Table { get; }

        public TEntity Get(TKey id);

        public TEntity Update(TEntity entity);

        public TEntity Add(TEntity entity);

        public void AddRange(List<TEntity> entities);

        public void Delete(TEntity entity);

        public void Delete(TKey id);

        public void SaveChanges();

        public Task SaveChangesAsync();

    }
}
