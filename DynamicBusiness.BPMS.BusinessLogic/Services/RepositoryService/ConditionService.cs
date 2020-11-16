using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicBusiness.BPMS.Domain;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class ConditionService : ServiceBase
    {
        public ConditionService(IUnitOfWork unitOfWork = null) : base(unitOfWork) { }

        public ResultOperation Add(sysBpmsCondition Condition)
        {
            ResultOperation resultOperation = new ResultOperation(Condition);
            if (resultOperation.IsSuccess)
            {
                Condition.Code = Condition.Code.ToStringObj().Trim();
                this.UnitOfWork.Repository<IConditionRepository>().Add(Condition);
                this.UnitOfWork.Save();
            }
            return resultOperation;
        }

        public ResultOperation Update(sysBpmsCondition Condition)
        {
            ResultOperation resultOperation = new ResultOperation();
            if (resultOperation.IsSuccess)
            {
                Condition.Code = Condition.Code.ToStringObj().Trim();

                this.UnitOfWork.Repository<IConditionRepository>().Update(Condition);
                this.UnitOfWork.Save();
            }
            return resultOperation;
        }

        public ResultOperation Delete(Guid ConditionId)
        {
            ResultOperation resultOperation = new ResultOperation(ConditionId);
            if (resultOperation.IsSuccess)
            {
                this.UnitOfWork.Repository<IConditionRepository>().Delete(ConditionId);
                this.UnitOfWork.Save();
            }
            return resultOperation;
        }

        public sysBpmsCondition GetInfo(Guid ID)
        {
            return this.UnitOfWork.Repository<IConditionRepository>().GetInfo(ID);
        }

        public List<sysBpmsCondition> GetList(Guid? gatewayID, Guid? sequenceFlowID, Guid? processId)
        {
            return this.UnitOfWork.Repository<IConditionRepository>().GetList(gatewayID, sequenceFlowID, processId);
        }
    }
}
