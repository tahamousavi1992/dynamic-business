using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    public interface IWorkflowElement
    {
        [NodeAttributeName("id")]
        string ID { get; }
        string ElementName { get; }
        string PersianElementName { get; }

        [NodeAttributeName("Name")]      
        string Name { get; }
    }
}
