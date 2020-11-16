using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    public interface ILUTableRepository
    {
        List<sysBpmsLUTable> GetList();
        sysBpmsLUTable GetInfo(Guid ID);
    }
}
