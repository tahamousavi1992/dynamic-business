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
               List<string> emailTo, string emailBcc, string emailCc, string Subject, string Body)
        {
            ResultOperation resultOperation = new ResultOperation();
            try
            {
                if (string.IsNullOrWhiteSpace(emailFrom) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(smtpAddress))
                {
                    resultOperation.AddError("Email Setting is not completed.");
                    return resultOperation;
                }
                bool enableSSL = true;
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(emailFrom);

                    foreach (var To in emailTo)
                        if (!string.IsNullOrWhiteSpace(To))
                            mail.To.Add(To.Trim());

                    foreach (var To in emailBcc.Split(','))
                        if (!string.IsNullOrWhiteSpace(To))
                            mail.Bcc.Add(To.Trim());

                    foreach (var To in emailCc.Split(','))
                        if (!string.IsNullOrWhiteSpace(To))
                            mail.CC.Add(To.Trim());

                    mail.Subject = Subject;
                    mail.Body = Body;
                    mail.IsBodyHtml = true;

                    using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
                    {
                        smtp.Host = smtpAddress;
                        smtp.Port = portNumber;
                        smtp.Credentials = new NetworkCredential(emailFrom, password);
                        smtp.EnableSsl = enableSSL;
                        smtp.Send(mail);
                    }
                }
            }
            catch (Exception Ex)
            {
                DotNetNuke.Services.Exceptions.Exceptions.LogException(Ex);
                resultOperation.AddError(Ex.ToString());
                return resultOperation;
            }
            return resultOperation;
        }


    }
}
