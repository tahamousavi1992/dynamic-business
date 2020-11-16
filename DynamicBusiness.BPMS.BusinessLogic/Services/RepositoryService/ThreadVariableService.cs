using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicBusiness.BPMS.Domain;
namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class ThreadVariableService : ServiceBase
    {
        public ThreadVariableService(IUnitOfWork unitOfWork = null) : base(unitOfWork) { }

        public ResultOperation Add(sysBpmsThreadVariable ThreadVariable)
        {
            ResultOperation resultOperation = new ResultOperation();
            if (resultOperation.IsSuccess)
            {
                this.UnitOfWork.Repository<IThreadVariableRepository>().Add(ThreadVariable);
                this.UnitOfWork.Save();
            }
            return resultOperation;
        }

        public ResultOperation Update(sysBpmsThreadVariable ThreadVariable)
        {
            ResultOperation resultOperation = new ResultOperation();
            if (resultOperation.IsSuccess)
            {
                this.UnitOfWork.Repository<IThreadVariableRepository>().Update(ThreadVariable);
                this.UnitOfWork.Save();
            }
            return resultOperation;
        }

        public ResultOperation Delete(Guid ID)
        {
            ResultOperation resultOperation = new ResultOperation();
            if (resultOperation.IsSuccess)
            {
                this.UnitOfWork.Repository<IThreadVariableRepository>().Delete(ID);
                this.UnitOfWork.Save();
            }
            return resultOperation;
        }

        public sysBpmsThreadVariable GetInfo(Guid ID)
        {
            return this.UnitOfWork.Repository<IThreadVariableRepository>().GetInfo(ID);
        }

        public sysBpmsThreadVariable GetInfo(Guid ThreadID, Guid VariableID)
        {
            return this.UnitOfWork.Repository<IThreadVariableRepository>().GetInfo(ThreadID, VariableID);
        }

        public List<sysBpmsThreadVariable> GetList(Guid? ThreadID, Guid? VariableID, string Value)
        {
            return this.UnitOfWork.Repository<IThreadVariableRepository>().GetList(ThreadID, VariableID, Value);
        }

    }
}
