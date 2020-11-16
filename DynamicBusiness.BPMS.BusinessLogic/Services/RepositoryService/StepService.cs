using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicBusiness.BPMS.Domain;
namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class StepService : ServiceBase
    {
        public StepService(IUnitOfWork unitOfWork = null) : base(unitOfWork) { }

        public ResultOperation Add(sysBpmsStep Step)
        {
            ResultOperation resultOperation = new ResultOperation();
            if (resultOperation.IsSuccess)
            {
                this.UnitOfWork.Repository<IStepRepository>().Add(Step);
                this.UnitOfWork.Save();
            }
            return resultOperation;
        }

        public ResultOperation Update(sysBpmsStep Step)
        {
            ResultOperation resultOperation = new ResultOperation();
            if (resultOperation.IsSuccess)
            {
                this.UnitOfWork.Repository<IStepRepository>().Update(Step);
                this.UnitOfWork.Save();
            }
            return resultOperation;
        }

        public ResultOperation Delete(Guid StepId)
        {
            ResultOperation resultOperation = new ResultOperation();
            if (resultOperation.IsSuccess)
            {
                this.UnitOfWork.Repository<IStepRepository>().Delete(StepId);
                this.UnitOfWork.Save();
            }
            return resultOperation;
        }

        public sysBpmsStep GetInfo(Guid ID, string[] Includes = null)
        {
            return this.UnitOfWork.Repository<IStepRepository>().GetInfo(ID, Includes);
        }
        /// <summary>
        /// Include DynamicForm and .OrderBy(c => c.Position)
        /// </summary>
        public List<sysBpmsStep> GetList(Guid? taskID, Guid? DynamicFormID)
        {
            return this.UnitOfWork.Repository<IStepRepository>().GetList(taskID, DynamicFormID);
        }
    }
}
