using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    public interface IDynamicCodeEngine
    {
        bool ExecuteBooleanCode(DesignCodeModel designCode, FormModel formModel);
    }
}
