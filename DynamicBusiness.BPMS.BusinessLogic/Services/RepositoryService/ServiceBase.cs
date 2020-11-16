using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicBusiness.BPMS.Domain;
namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class ServiceBase : IDisposable
    {
        private bool ManageTransaction { get; set; }

        public IUnitOfWork UnitOfWork { get; set; }

        public ServiceBase(IUnitOfWork unitOfWork = null)
        {
            this.UnitOfWork = unitOfWork ?? new UnitOfWork();
            this.ManageTransaction = !this.UnitOfWork.BeginnedTransaction;
        }

        public void BeginTransaction()
        {
            if (this.ManageTransaction)
                this.UnitOfWork.BeginTransaction();
        }

        public void CommitTransaction()
        {
            if (this.ManageTransaction)
                this.UnitOfWork.Commit();
        }

        public void RollbackTransaction()
        {
            if (this.ManageTransaction)
                this.UnitOfWork.Rollback();
        }
        public ResultOperation ExceptionHandler(Exception exception)
        {
            this.RollbackTransaction();
            ResultOperation resultOperation = new ResultOperation();
            if (exception.InnerException == null && !string.IsNullOrWhiteSpace(exception.Message))
                resultOperation.AddError(exception.Message);
            else
                resultOperation.AddError(exception.ToString());
            return resultOperation;
        }

        protected void FinalizeService(ResultOperation resultOperation)
        {
            if (resultOperation.IsSuccess)
            {
                this.CommitTransaction();
            }
            else this.RollbackTransaction();
        }

        public void Dispose()
        {
            UnitOfWork.Dispose();
        }
    }
}
