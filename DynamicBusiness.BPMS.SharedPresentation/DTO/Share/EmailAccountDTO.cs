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
    public class EmailAccountDTO
    {
        public EmailAccountDTO() { }
        public EmailAccountDTO(sysBpmsEmailAccount emailAccount)
        {
            if (emailAccount != null)
            {
                this.ID = emailAccount.ID;
                this.ObjectID = emailAccount.ObjectID;
                this.ObjectTypeLU = emailAccount.ObjectTypeLU;
                this.Email = emailAccount.Email;
                this.SMTP = emailAccount.SMTP;
                this.Port = emailAccount.Port;
                this.MailUserName = emailAccount.MailUserName;
                this.MailPassword = emailAccount.MailPassword;
            }
        }
        [DataMember]
        public Guid ID { get; set; }
        [DataMember]
        public int ObjectTypeLU { get; set; }
        [DataMember]
        public Nullable<System.Guid> ObjectID { get; set; }
         
        [DataMember]
        public string SMTP { get; set; }
         
        [DataMember]
        public string Port { get; set; }
         
        [DataMember]
        public string MailUserName { get; set; }
         
        [DataType(DataType.Password)]
        [DataMember]
        public string MailPassword { get; set; }
         
        [DataMember]
        public string Email { get; set; }
    }
}