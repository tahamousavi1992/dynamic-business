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
    public class ProcessGroupDTO
    {
        public ProcessGroupDTO() { }
        public ProcessGroupDTO(sysBpmsProcessGroup processGroup)
        {
            if (processGroup != null)
            {
                this.ID = processGroup.ID;
                this.ProcessGroupID = processGroup.ProcessGroupID;
                this.Name = processGroup.Name;
                this.Description = processGroup.Description;
            }
        }
        [DataMember]
        public Guid ID { get; set; }
         
        [DataMember]
        public Guid? ProcessGroupID { get; set; }

        [Required] 
        [DataMember]
        public string Name { get; set; }
  
        [DataMember]
        public string Description { get; set; }

    }
}