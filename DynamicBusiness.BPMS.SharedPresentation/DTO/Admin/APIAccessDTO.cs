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
    public class APIAccessDTO
    {
        public APIAccessDTO() { }
        public APIAccessDTO(sysBpmsAPIAccess apiAccess)
        {
            if (apiAccess != null)
            {
                this.ID = apiAccess.ID;
                this.Name = apiAccess.Name;
                this.IPAddress = apiAccess.IPAddress;
                this.AccessKey = apiAccess.AccessKey;
                this.IsActive = apiAccess.IsActive;
            }
        }
        [DataMember]
        public Guid ID { get; set; }
        [Required]
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string IPAddress { get; set; }
        [Required]
        [DataMember]
        public string AccessKey { get; set; }
        [DataMember]
        public bool IsActive { get; set; }
    }
}