using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    [Serializable]
    public partial class sysBpmsStep
    {
        public sysBpmsStep() { }
        public sysBpmsStep(Guid id, Guid taskID, int position, Guid? dynamicFormID, string name)
        {
            this.ID = id;
            this.TaskID = taskID;
            this.Position = position;
            this.DynamicFormID = dynamicFormID;
            this.Name = name;
        }
        
    }
}
