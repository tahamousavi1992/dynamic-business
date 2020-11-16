using DynamicBusiness.BPMS.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace DynamicBusiness.BPMS.SharedPresentation
{
    [DataContract]
    public class StepDTO
    {
        public StepDTO() { }
        public StepDTO(sysBpmsStep step)
        {
            this.ID = step.ID;
            this.TaskID = step.TaskID;
            this.Position = step.Position;
            this.DynamicFormID = step.DynamicFormID;
            this.Name = step.Name;
        }
        [DataMember]
        public Guid ID { get; set; }
        [DataMember] 
        public Guid TaskID { get; set; }
        [DataMember] 
        public int Position { get; set; } 
        [DataMember]
        public Nullable<Guid> DynamicFormID { get; set; } 
        [DataMember]
        public string Name { get; set; }
    }
}