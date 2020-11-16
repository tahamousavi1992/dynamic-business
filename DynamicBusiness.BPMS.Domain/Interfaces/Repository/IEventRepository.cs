using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    public interface IEventRepository
    {
        void Add(sysBpmsEvent Event);
        void Update(sysBpmsEvent Event);
        void Delete(Guid id);
        sysBpmsEvent GetInfo(Guid id);
        sysBpmsEvent GetInfo(string elementId, Guid processId);
        List<sysBpmsEvent> GetList(int? TypeLU, Guid? ProcessID, string RefElementID, int? SubType, string[] Includes);
        List<sysBpmsEvent> GetList(int? TypeLU, Guid? ProcessID, string RefElementID, int? SubType, int? ProcessStatusLU, string[] Includes);
        List<sysBpmsEvent> GetListStartMessage(Guid? notProcessID, string key, Guid messageTypeID, string[] Includes);
 
    }
}
