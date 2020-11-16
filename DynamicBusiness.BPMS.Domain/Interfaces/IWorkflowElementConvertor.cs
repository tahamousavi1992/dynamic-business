using DynamicBusiness.BPMS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public interface IWorkflowElementConvertor<TWorkflowElement> where TWorkflowElement : IWorkflowElement
    {
        string ConvertToString(TWorkflowElement node);

        TWorkflowElement ConvertFromString(string data);
    }
}
