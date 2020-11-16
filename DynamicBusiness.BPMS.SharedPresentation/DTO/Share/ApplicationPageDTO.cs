using DynamicBusiness.BPMS.BusinessLogic;
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
    public class ApplicationPageDTO
    {
        public ApplicationPageDTO() { }
        public ApplicationPageDTO(sysBpmsApplicationPage applicationPage)
        {
            this.ID = applicationPage.ID;
            this.Description = applicationPage.Description;
            this.GroupLU = applicationPage.GroupLU;
            this.ShowInMenu = applicationPage.ShowInMenu;
        }
        [DataMember]
        public System.Guid ID { get; set; } 
        [DataType(DataType.MultilineText)]
        [DataMember]
        public string Description { get; set; }

        [Required] 
        [DataMember]
        public int GroupLU { get; set; }
         
        [DataMember]
        public bool ShowInMenu { get; set; }
        [DataMember]

        public List<LURowDTO> ListGroupLU = null;
    }
}