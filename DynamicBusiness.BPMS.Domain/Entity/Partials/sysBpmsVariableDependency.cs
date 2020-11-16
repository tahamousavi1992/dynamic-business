using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    public partial class sysBpmsVariableDependency
    {
        public sysBpmsVariableDependency() { }
        public ResultOperation Update(Guid id, Guid dependentVariableId, string dependentPropertyName, Guid? toVariableID, string toPropertyName, string description)
        {
            this.ID = id;
            this.DependentVariableID = dependentVariableId;
            this.DependentPropertyName = dependentPropertyName;
            this.ToVariableID = toVariableID;
            this.ToPropertyName = toPropertyName;
            this.Description = description;

            ResultOperation resultOperation = new ResultOperation(this);
            if (string.IsNullOrWhiteSpace(this.DependentPropertyName))
            {
                resultOperation.AddError(SharedLang.GetReuired(nameof(sysBpmsVariableDependency.DependentPropertyName), nameof(sysBpmsVariableDependency)));
            }
            if (this.ToVariableID.ToGuidObj() == Guid.Empty)
            {
                resultOperation.AddError(SharedLang.GetReuired(nameof(sysBpmsVariableDependency.ToVariableID), nameof(sysBpmsVariableDependency)));
            }
            return resultOperation;
        }
        public void Load(sysBpmsVariableDependency variableDependency)
        {
            this.ID = variableDependency.ID;
            this.DependentVariableID = variableDependency.DependentVariableID;
            this.DependentPropertyName = variableDependency.DependentPropertyName;
            this.ToVariableID = variableDependency.ToVariableID;
            this.ToPropertyName = variableDependency.ToPropertyName;
            this.Description = variableDependency.Description;
        }
    }
}
