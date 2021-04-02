using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    public interface IMessageCodeHelper
    {
        /// <summary>
        /// it is used to show an error message at the end to the client 
        /// </summary>
        void AddError(string message);

        /// <summary>
        /// it is used to show an info message at the end to the client 
        /// </summary>
        void AddInfo(string message);

        /// <summary>
        /// it is used to show a success message at the end to the client 
        /// </summary>
        void AddSuccess(string message);
        /// <summary>
        /// it is used to show a warning message at the end to the client 
        /// </summary>
        void AddWarning(string message);

        /// <summary>
        /// It is used to send an email using emailAccountID.
        /// </summary>
        bool SendEmail(string from, string smtpAddress, string password, int port,
            List<string> to, string bcc, string cc, string subject, string body);

        /// <summary>
        /// It is used to send an email. 
        /// </summary>
        bool SendEmail(Guid emailAccountID,
            List<string> to, string bcc, string cc, string subject, string body);
    }
}
