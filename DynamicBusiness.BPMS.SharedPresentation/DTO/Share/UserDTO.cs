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
    public class UserDTO
    {
        public UserDTO() { }
        public UserDTO(sysBpmsUser user, sysBpmsEmailAccount emailAccount = null)
        {
            if (user != null)
            {
                this.ID = user.ID;
                this.Username = user.Username;
                this.FirstName = user.FirstName;
                this.LastName = user.LastName; 
                this.Email = user.Email;
                this.Tel = user.Tel; 
                this.Mobile = user.Mobile;
                this.EmailAccountDTO = new EmailAccountDTO(emailAccount);
            }
        }

        [DataMember]
        public Guid ID { get; set; }
        [Required] 
        [DataMember]
        public string Username { get; set; }
        [Required] 
        [DataMember]
        public string FirstName { get; set; } 
        [DataMember]
        public string LastName { get; set; } 
        [DataMember]
        public string Email { get; set; } 
        [DataMember]
        public string Tel { get; set; } 
        [DataMember]
        public string Mobile { get; set; }
        [DataMember]
        public EmailAccountDTO EmailAccountDTO { get; set; }
         
        [DataMember]
        public string FullName { get { return this.FirstName + " " + this.LastName; } private set { } }
         
        [DataType(DataType.Password)]
        [DataMember]
        public string Password { get; set; }
    }
}