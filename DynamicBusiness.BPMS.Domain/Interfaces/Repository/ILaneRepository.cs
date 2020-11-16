using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    public interface ILaneRepository
    {
        void Add(sysBpmsLane Lane);
        void Update(sysBpmsLane Lane);
        void Delete(Guid id);
        sysBpmsLane GetInfo(Guid id);
        List<sysBpmsLane> GetList(Guid? ProcessID);
    }
}
