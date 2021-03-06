﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    public partial class sysBpmsElement
    {
       
        public enum e_TypeLU
        {
            Task = 1,
            Gateway = 2,
            Event = 3,
            Lane = 4,
            Sequence = 5,
        }

        public sysBpmsElement Clone()
        {
            return new sysBpmsElement
            {
                ID = this.ID,
                ProcessID = this.ProcessID,
                Name = this.Name,
                TypeLU = this.TypeLU,
            };
        }
    }
}
