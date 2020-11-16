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
    public class LUTableDTO
    {
        public LUTableDTO() { }
        public LUTableDTO(sysBpmsLUTable sysBpmsLUTable)
        {
            if (sysBpmsLUTable != null)
            {
                this.ID = sysBpmsLUTable.ID;
                this.NameOf = sysBpmsLUTable.NameOf;
                this.Alias = sysBpmsLUTable.Alias;
                this.IsSystemic = sysBpmsLUTable.IsSystemic;
                this.IsActive = sysBpmsLUTable.IsActive;
            }
        }

        [DataMember]
        public Guid ID { get; set; }
         
        [DataMember]
        [Required]
        public string NameOf { get; set; }
         
        [DataMember]
        public string Alias { get; set; }
         
        [DataMember]
        public bool IsSystemic { get; set; }
         
        [DataMember]
        public bool IsActive { get; set; }
    }
}