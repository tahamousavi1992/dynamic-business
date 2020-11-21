using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    public interface ICodeBase
    {
        IVariableCodeHelper VariableHelper { get; }
    }
}
