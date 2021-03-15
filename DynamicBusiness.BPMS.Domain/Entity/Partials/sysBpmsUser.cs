using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    [Serializable]
    public partial class sysBpmsUser
    {
        [NotMapped]
        public string FullName
        {
            get { return this.FirstName + " " + this.LastName; }
        }
        public sysBpmsUser(Guid ID, string username, string firstName,
            string lastName, string email, string tel, string mobile)
        {
            this.ID = ID;
            this.Username = username;
            this.FirstName = firstName.ToStringObj().Trim();
            this.LastName = lastName.ToStringObj().Trim();
            this.Email = email;
            this.Tel = tel;
            this.Mobile = mobile;
        }
        
    }
}
