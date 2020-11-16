using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    public abstract class WorkflowElement : IWorkflowElement
    {
        [Required]
        public string ID { get; set; }

        public string ElementName { get; set; }

        public string PersianElementName { get; set; }

        [DisplayName("نام")]
        public string Name { get; set; }

        public void Initialize(string id, string name)
        {
            if (!string.IsNullOrEmpty(id))
                ID = id;
            if (!string.IsNullOrEmpty(name))
                Name = name;
        }

        public WorkflowElement(string id, string name = "")
        {
            ID = id;
            Name = name;
        }
    }
}
