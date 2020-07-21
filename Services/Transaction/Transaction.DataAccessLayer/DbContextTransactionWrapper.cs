using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace Transaction.Models
{
    public interface IDbContextTransactionWrapper : IDisposable
    {
        void Commit();

        void Rollback();
    }

    class DbContextTransactionWrapper : IDbContextTransactionWrapper
    {
        private IDbContextTransaction _transaction;
        public DbContextTransactionWrapper(IDbContextTransaction transaction)
        {
            _transaction = transaction;
        }

        public void Commit()
        {
            _transaction.Commit();
        }

        public void Dispose()
        {
            _transaction.Dispose();
        }

        public void Rollback()
        {
            _transaction.Rollback();
        }
    }
}
