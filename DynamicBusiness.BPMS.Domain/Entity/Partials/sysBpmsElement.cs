using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    public partial class sysBpmsElement
    {
        public void Load(sysBpmsElement Element)
        {
            this.ID = Element.ID;
            this.ProcessID = Element.ProcessID;
            this.Name = Element.Name;
            this.TypeLU = Element.TypeLU;
        }
        public enum e_TypeLU
        {
            Task = 1,
            Gateway = 2,
            Event = 3,
            Lane = 4,
            Sequence = 5,
        }
    }
}
