using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    public interface IGatewayRepository
    {
        void Add(sysBpmsGateway gateway);
        void Update(sysBpmsGateway gateway);
        void Delete(Guid id);
        sysBpmsGateway GetInfo(Guid id);
        sysBpmsGateway GetInfo(string elementId, Guid processId);
        List<sysBpmsGateway> GetList(Guid processID);
        List<sysBpmsGateway> GetListByDefaultSequence(Guid defaultSequenceFlowID);
    }
}
