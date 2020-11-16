using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    public interface ISequenceFlowRepository
    {
        void Add(sysBpmsSequenceFlow SequenceFlow);
        void Update(sysBpmsSequenceFlow SequenceFlow);
        void Delete(Guid id);
        sysBpmsSequenceFlow GetInfo(Guid id);
        sysBpmsSequenceFlow GetInfo(string elementId, Guid processId);
        List<sysBpmsSequenceFlow> GetList(Guid ProcessID, string TargetElementID, string SourceElementID, string Name);
        List<sysBpmsSequenceFlow> GetList(Guid ProcessID);
    }
}
