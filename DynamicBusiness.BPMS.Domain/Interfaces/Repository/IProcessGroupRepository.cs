using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    public interface IProcessGroupRepository
    {
        void Add(sysBpmsProcessGroup processGroup);
        void Update(sysBpmsProcessGroup processGroup);
        void Delete(Guid processGroupId);
        sysBpmsProcessGroup GetInfo(Guid id);
        List<sysBpmsProcessGroup> GetList(string name, Guid? parentProcessGroupID);
    }
}
