﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transaction.Core.Interfaces;
using Transaction.Domain;

namespace Transaction.Infrastructure.Data
{
    class GenericRepository<TKey, TEntity> : IRepository<TKey, TEntity> where TEntity : BaseEntity<TKey>
    {
        private TransactionContext _context;

        public IQueryable<TEntity> Table => _context.Set<TEntity>();


        public GenericRepository(TransactionContext context)
        {
            _context = context;
        }

        public TEntity Add(TEntity entity)
        {
            return _context.Set<TEntity>()
                .Add(entity).Entity;
        }

        public void Delete(TEntity entity)
        {
            _context.Set<TEntity>()
                .Remove(entity);
        }

        /// <summary>
        /// Not For composit keys
        /// </summary>
        /// <param name="id"></param>
        public void Delete(TKey id)
        {
            var entity = _context.Set<TEntity>().Find(id);
            _context.Set<TEntity>()
                .Remove(entity);

        }

        /// <summary>
        /// Not for composit keys
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TEntity Get(TKey id)
        {
            return _context.Set<TEntity>().Find(id);

        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public TEntity Update(TEntity entity)
        {
            _context.Entry(entity).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            return entity;
        }

        public void AddRange(List<TEntity> entities)
        {
            _context.AddRange(entities);
        }
    }
}
