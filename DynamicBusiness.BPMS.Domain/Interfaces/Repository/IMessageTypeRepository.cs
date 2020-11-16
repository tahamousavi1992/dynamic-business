using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    public interface IMessageTypeRepository
    {
        void Add(sysBpmsMessageType sysBpmsMessageType);
        void Update(sysBpmsMessageType sysBpmsMessageType);
        void Delete(Guid messageTypeId);
        sysBpmsMessageType GetInfo(Guid id, string[] Includes);
        sysBpmsMessageType GetInfo(string name, string[] Includes);
        List<sysBpmsMessageType> GetList(string name, bool? isActive, PagingProperties currentPaging);
    }
}
