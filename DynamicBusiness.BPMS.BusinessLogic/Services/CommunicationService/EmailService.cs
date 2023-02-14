using DynamicBusiness.BPMS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class EmailService
    {

        public ResultOperation SendEmailAsync(string emailFrom, string password, string smtpAddress, int portNumber,
            List<string> emailTo, string emailBcc, string emailCc, string Subject, string Body)
        {
            Thread t = new Thread(new ThreadStart(() =>
            {
                SendEmail(emailFrom, password, smtpAddress, portNumber,
             emailTo, emailBcc, emailCc, Subject, Body);
            }));
            t.Start();
            return new ResultOperation();
        }

        public ResultOperation SendEmail(string emailFrom, string password, string smtpAddress, int portNumber,
               List<string> emailTo, string emailBcc, string emailCc, string subject, string body)
        {
            ResultOperation resultOperation = new ResultOperation();
            if (string.IsNullOrWhiteSpace(emailFrom) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(smtpAddress))
            {
                resultOperation.AddError("Email setting is incomplete.");
                return resultOperation;
            }

            var message = new MailMessage();
            message.From = new MailAddress(emailFrom);

            foreach (var to in emailTo.Where(e => !string.IsNullOrWhiteSpace(e)))
            {
                message.To.Add(to.Trim());
            }

            foreach (var bcc in emailBcc.Split(',').Where(e => !string.IsNullOrWhiteSpace(e)))
            {
                message.Bcc.Add(bcc.Trim());
            }

            foreach (var cc in emailCc.Split(',').Where(e => !string.IsNullOrWhiteSpace(e)))
            {
                message.CC.Add(cc.Trim());
            }

            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;

            var smtpClient = new SmtpClient(smtpAddress, portNumber);
            smtpClient.Credentials = new NetworkCredential(emailFrom, password);
            smtpClient.EnableSsl = true;

            try
            {
                smtpClient.Send(message);
            }
            catch (Exception ex)
            {
                DotNetNuke.Services.Exceptions.Exceptions.LogException(ex);
                resultOperation.AddError(ex.ToString());
                return resultOperation;
            }

            return resultOperation;
        }


    }
}
