using DynamicBusiness.BPMS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class MessageCodeHelper : IMessageCodeHelper
    {
        public CodeBaseSharedModel CodeBaseShared { get; set; }
        public IUnitOfWork UnitOfWork { get; set; }
        public MessageCodeHelper(CodeBaseSharedModel codeBaseShared, IUnitOfWork unitOfWork)
        {
            this.CodeBaseShared = codeBaseShared;
            this.UnitOfWork = unitOfWork;
        }

        /// <summary>
        /// it is used to show an error message at the end to the client 
        /// </summary>
        public void AddError(string message)
        {
            this.CodeBaseShared.MessageList.Add(new MessageModel(DisplayMessageType.error, message));
        }

        /// <summary>
        /// it is used to show an info message at the end to the client 
        /// </summary>
        public void AddInfo(string message)
        {
            this.CodeBaseShared.MessageList.Add(new MessageModel(DisplayMessageType.info, message));
        }

        /// <summary>
        /// it is used to show a success message at the end to the client 
        /// </summary>
        public void AddSuccess(string message)
        {
            this.CodeBaseShared.MessageList.Add(new MessageModel(DisplayMessageType.success, message));
        }

        /// <summary>
        /// it is used to show a warning message at the end to the client 
        /// </summary>
        public void AddWarning(string message)
        {
            this.CodeBaseShared.MessageList.Add(new MessageModel(DisplayMessageType.warning, message));
        }


        /// <summary>
        /// It is used to send an email. 
        /// </summary>
        public bool SendEmail(string from, string smtpAddress, string password, int port,
            List<string> to, string bcc, string cc, string subject, string body)
        {
            return new EmailService().SendEmailAsync(from, password, smtpAddress, port, to, bcc, cc, subject, body).IsSuccess;
        }

        /// <summary>
        /// It is used to send an email using emailAccountID.
        /// </summary>
        public bool SendEmail(Guid emailAccountID,
            List<string> to, string bcc, string cc, string subject, string body)
        {
            sysBpmsEmailAccount account = new EmailAccountService(this.UnitOfWork).GetInfo(emailAccountID);
            return new EmailService().SendEmailAsync(account.Email, account.MailPassword, account.SMTP, account.Port.ToIntObj(), to, bcc, cc, subject, body).IsSuccess;
        } 
    }
}
