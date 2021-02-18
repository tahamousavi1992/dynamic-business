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
        public sysBpmsUser(Guid ID, string Username, string FirstName,
            string LastName, string Email, string Tel, string Mobile)
        {
            this.ID = ID;
            this.Username = Username;
            this.FirstName = FirstName;
            this.LastName = LastName;
            this.Email = Email;
            this.Tel = Tel;
            this.Mobile = Mobile;
        }
        
    }
}
