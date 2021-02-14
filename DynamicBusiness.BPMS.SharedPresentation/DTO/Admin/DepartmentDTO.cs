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
    public class DepartmentDTO
    {
        public DepartmentDTO() { }
        public DepartmentDTO(sysBpmsDepartment department, sysBpmsEmailAccount emailAccount = null)
        {
            if (department != null)
            {
                this.ID = department.ID;
                this.DepartmentID = department.DepartmentID;
                this.Name = department.Name;
                this.IsActive = department.IsActive;
                this.SMTP = emailAccount?.SMTP;
                this.Port = emailAccount?.Port;
                this.MailPassword = emailAccount?.MailPassword;
                this.WorkEmail = emailAccount?.Email;
            }
        }
        [DataMember]
        public Guid ID { get; set; }
         
        [DataMember]
        public Nullable<Guid> DepartmentID { get; set; }

        [Required] 
        [DataMember]
        public string Name { get; set; }
         
        [DataMember]
        public bool? IsActive { get; set; }
         
        [DataMember]
        public string SMTP { get; set; }
         
        [DataMember]
        public string Port { get; set; }
         
        [DataMember]
        public string MailPassword { get; set; }
         
        [DataMember]
        public string WorkEmail { get; set; }
    }
}