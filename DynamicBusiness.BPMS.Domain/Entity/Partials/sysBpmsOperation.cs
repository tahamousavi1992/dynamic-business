using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    [Serializable]
    public partial class sysBpmsOperation
    {
        public ResultOperation Update(int groupLU, int typeLU, string sqlCommand, string name)
        {
            this.GroupLU = groupLU;
            this.SqlCommand = sqlCommand;
            this.Name = name;
            this.TypeLU = typeLU;

            ResultOperation resultOperation = new ResultOperation(this);
            if (string.IsNullOrWhiteSpace(this.SqlCommand))
                resultOperation.AddError(SharedLang.GetReuired(nameof(sysBpmsOperation.SqlCommand), nameof(sysBpmsOperation)));
            if (string.IsNullOrWhiteSpace(this.Name))
                resultOperation.AddError(SharedLang.GetReuired(nameof(sysBpmsOperation.Name), nameof(sysBpmsOperation)));
            return resultOperation;

        }

        public void Load(sysBpmsOperation operation)
        {
            this.ID = operation.ID;
            this.GroupLU = operation.GroupLU;
            this.SqlCommand = operation.SqlCommand;
            this.Name = operation.Name;
            this.TypeLU = operation.TypeLU;
        }

        public enum e_TypeLU
        {
            [Description("Execute")]
            Execute = 1,
            [Description("Scalar")]
            Scalar = 2,
            [Description("DataTable")]
            DataTable = 3,
        }
    }
}
