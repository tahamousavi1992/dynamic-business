using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    public interface IUnitOfWork : IDisposable
    {
        bool BeginnedTransaction { get; }
        TEntity Repository<TEntity>() where TEntity : class;
        void BeginTransaction();
        void Commit();
        void Rollback();
        void Save();
    }
}
