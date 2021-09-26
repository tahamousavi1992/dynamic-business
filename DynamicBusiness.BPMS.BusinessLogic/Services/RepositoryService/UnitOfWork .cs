using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicBusiness.BPMS.Domain;
namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class UnitOfWork : IUnitOfWork
    {
        private Db_BPMSEntities Context { get; set; }
        private DbContextTransaction Transaction { get; set; }
        private Hashtable _repositories;
        private bool beginnedTransaction { get; set; }
        private bool disposed = false;


        public bool BeginnedTransaction { get { return this.beginnedTransaction; } }
        public UnitOfWork()
        {
            this.Context = new Db_BPMSEntities();
        }
        public UnitOfWork(Db_BPMSEntities db_BPMS)
        {
            this.Context = db_BPMS;
        }
        public TEntity Repository<TEntity>() where TEntity : class
        {
            if (_repositories == null)
                _repositories = new Hashtable();

            string TypeName = typeof(TEntity).Name;

            if (_repositories.ContainsKey(TypeName)) return (TEntity)_repositories[TypeName];
            _repositories.Add(TypeName, RepositoryFactory<TEntity>.Create(this.Context));

            return (TEntity)_repositories[TypeName];
        }

        public TEntity Repository<TEntity>(object TFactory) where TEntity : class
        {
            if (_repositories == null)
                _repositories = new Hashtable();

            string TypeName = typeof(TEntity).Name;

            if (_repositories.ContainsKey(TypeName)) return (TEntity)_repositories[TypeName];
            _repositories.Add(TypeName, TFactory);

            return (TEntity)_repositories[TypeName];
        }

        public void BeginTransaction()
        {
            this.beginnedTransaction = true;
            this.Transaction = this.Context.Database.BeginTransaction();
        }
        public void Commit()
        {
            try
            {
                this.beginnedTransaction = false;
                // commit transaction if there is one active
                if (this.Transaction != null)
                    this.Transaction.Commit();
            }
            catch
            {
                // rollback if there was an exception
                if (this.Transaction != null)
                    this.Transaction.Rollback();

                throw;
            }
            finally
            {
                this.Transaction?.Dispose();
            }
        }
        public void Rollback()
        {
            try
            {
                this.beginnedTransaction = false;
                if (this.Transaction != null && this.Transaction.UnderlyingTransaction.Connection != null)
                    this.Transaction.Rollback();
            }
            finally
            {
                if (this.Transaction != null)
                    this.Transaction.Dispose();
            }
        }
        public void Save()
        {
            this.Context.SaveChanges();
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    Context.Dispose();
                }
            }
            this.disposed = true;
        }
    }
}
