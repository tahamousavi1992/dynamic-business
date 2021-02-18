using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    [Serializable]
    public partial class sysBpmsEmailAccount
    {
        public sysBpmsEmailAccount()
        {

        }
        public enum e_ObjectTypeLU
        {
            Systemic = 1,
            Department = 2,
            User = 3,
        }
        public ResultOperation Update(int ObjectTypeLU, Guid? ObjectID, string smtp, string port, string mailPassword, string email)
        {
            this.ObjectTypeLU = ObjectTypeLU;
            this.ObjectID = ObjectID;
            this.SMTP = smtp;
            this.Port = port; 
            this.MailPassword = mailPassword;
            this.Email = email;
            ResultOperation resultOperation = new ResultOperation(this);
            if (!string.IsNullOrWhiteSpace(this.Email + this.MailPassword  + this.SMTP))
            { 
                if (string.IsNullOrWhiteSpace(this.Email))
                    resultOperation.AddError(SharedLang.GetReuired(nameof(sysBpmsEmailAccount.Email), nameof(sysBpmsEmailAccount)));
                if (string.IsNullOrWhiteSpace(this.SMTP))
                    resultOperation.AddError(SharedLang.GetReuired(nameof(sysBpmsEmailAccount.SMTP), nameof(sysBpmsEmailAccount)));
                if (string.IsNullOrWhiteSpace(this.Port))
                    resultOperation.AddError(SharedLang.GetReuired(nameof(sysBpmsEmailAccount.Port), nameof(sysBpmsEmailAccount)));
                if (string.IsNullOrWhiteSpace(this.MailPassword))
                    resultOperation.AddError(SharedLang.GetReuired(nameof(sysBpmsEmailAccount.MailPassword), nameof(sysBpmsEmailAccount)));
            }
            return resultOperation;
        }

        
    }
}
