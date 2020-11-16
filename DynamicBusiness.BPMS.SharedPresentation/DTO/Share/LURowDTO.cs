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
    public class LURowDTO
    {
        public LURowDTO() { }
        public LURowDTO(sysBpmsLURow lURow)
        {
            if (lURow != null)
            {
                this.ID = lURow.ID;
                this.LUTableID = lURow.LUTableID;
                this.NameOf = lURow.NameOf;
                this.CodeOf = lURow.CodeOf;
                this.DisplayOrder = lURow.DisplayOrder;
                this.IsSystemic = lURow.IsSystemic;
                this.IsActive = lURow.IsActive;
            }
        }

        [DataMember]
        public Guid ID { get; set; }
        [DataMember]
        public System.Guid LUTableID { get; set; }
         
        [DataMember]
        [Required]
        public string NameOf { get; set; }
         
        [DataMember]
        public string CodeOf { get; set; }
         
        [DataMember]
        public int DisplayOrder { get; set; }
         
        [DataMember]
        public bool IsSystemic { get; set; }
         
        [DataMember]
        public bool IsActive { get; set; }
    }
}