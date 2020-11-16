using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    public interface IStepRepository
    {
        void Add(sysBpmsStep Step);
        void Update(sysBpmsStep Step);
        void Delete(Guid StepId);
        sysBpmsStep GetInfo(Guid ID, string[] Includes);
        List<sysBpmsStep> GetList(Guid? taskID, Guid? DynamicFormID);
    }
}
