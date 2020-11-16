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
        public void Load(sysBpmsStep Step)
        {
            this.ID = Step.ID;
            this.TaskID = Step.TaskID;
            this.DynamicFormID = Step.DynamicFormID;
            this.Position = Step.Position;
            this.Name = Step.Name;
        }
    }
}
