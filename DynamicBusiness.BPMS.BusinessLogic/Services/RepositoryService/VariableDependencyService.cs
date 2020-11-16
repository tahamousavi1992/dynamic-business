using DynamicBusiness.BPMS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class VariableDependencyService : ServiceBase
    {
        public VariableDependencyService(IUnitOfWork unitOfWork = null) : base(unitOfWork) { }

        public ResultOperation Add(sysBpmsVariableDependency variableDependency)
        {
            ResultOperation resultOperation = new ResultOperation();
            if (string.IsNullOrWhiteSpace(variableDependency.ToPropertyName))
            {
                if (new VariableService(base.UnitOfWork).GetInfo(variableDependency.ToVariableID.Value).VarTypeLU == (int)sysBpmsVariable.e_VarTypeLU.Object)
                {
                    resultOperation.AddError(SharedLang.GetReuired(nameof(sysBpmsVariableDependency.ToPropertyName), nameof(sysBpmsVariableDependency)));
                }
            }
            if (resultOperation.IsSuccess)
            {
                this.UnitOfWork.Repository<IVariableDependencyRepository>().Add(variableDependency);
                this.UnitOfWork.Save();
            }
            return resultOperation;
        }

        public ResultOperation Update(sysBpmsVariableDependency variableDependency)
        {
            ResultOperation resultOperation = new ResultOperation();
            if (string.IsNullOrWhiteSpace(variableDependency.ToPropertyName))
            {
                if (new VariableService(base.UnitOfWork).GetInfo(variableDependency.ToVariableID.Value).VarTypeLU == (int)sysBpmsVariable.e_VarTypeLU.Object)
                {
                    resultOperation.AddError(SharedLang.GetReuired(nameof(sysBpmsVariableDependency.ToPropertyName), nameof(sysBpmsVariableDependency)));
                }
            }
            if (resultOperation.IsSuccess)
            {
                this.UnitOfWork.Repository<IVariableDependencyRepository>().Update(variableDependency);
                this.UnitOfWork.Save();
            }
            return resultOperation;
        }

        public ResultOperation Delete(Guid variableDependencyId)
        {
            ResultOperation resultOperation = new ResultOperation();

            this.UnitOfWork.Repository<IVariableDependencyRepository>().Delete(variableDependencyId);
            this.UnitOfWork.Save();

            return resultOperation;

        }

        public sysBpmsVariableDependency GetInfo(Guid ID)
        {
            return this.UnitOfWork.Repository<IVariableDependencyRepository>().GetInfo(ID);
        }

        public List<sysBpmsVariableDependency> GetList(Guid? DependentVariableID, Guid? ToVariableID)
        {
            return this.UnitOfWork.Repository<IVariableDependencyRepository>().GetList(DependentVariableID, ToVariableID);
        }

        public List<sysBpmsVariableDependency> GetList(Guid processId)
        {
            return this.UnitOfWork.Repository<IVariableDependencyRepository>().GetList(processId);
        }

    }
}
