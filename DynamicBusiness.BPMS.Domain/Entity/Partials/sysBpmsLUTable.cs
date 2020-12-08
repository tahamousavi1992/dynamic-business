
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace DynamicBusiness.BPMS.Domain
{
    [Serializable]
    public partial class sysBpmsLUTable
    {
        public enum e_LUTable
        {
            DepartmentRoleLU,
            LaneOwnerTypeLU,
            ProcessStatusLU,
            ApplicationPageGroupLU,
        }

    }
}
