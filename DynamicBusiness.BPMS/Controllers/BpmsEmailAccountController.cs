using DynamicBusiness.BPMS.BusinessLogic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using DynamicBusiness.BPMS.Domain;
using DynamicBusiness.BPMS.SharedPresentation;

namespace DynamicBusiness.BPMS.Controllers
{
    public class BpmsEmailAccountController : BpmsAdminApiControlBase
    {
        [HttpGet]
        public object GetList([System.Web.Http.FromUri] EmailAccountIndexSearchDTO indexSearchVM)
        {
            //base.SetMenuIndex(AdminMenuIndex.EmailAccountIndex);
            using (EmailAccountService emailAccountService = new EmailAccountService())
            {
                List<sysBpmsEmailAccount> list = emailAccountService.GetList((int)sysBpmsEmailAccount.e_ObjectTypeLU.Systemic, null, indexSearchVM.GetPagingProperties);
                indexSearchVM.Update(list.Select(c => new EmailAccountDTO(c)).ToList());
                return indexSearchVM;
            }
        }

        [HttpGet]
        public object GetAddEdit(Guid? ID = null)
        {
            return new EmailAccountDTO(ID.HasValue ? new EmailAccountService().GetInfo(ID.Value) : new sysBpmsEmailAccount());
        }

        [HttpPost]
        public object PostAddEdit(EmailAccountDTO EmailAccountDTO)
        {
            if (ModelState.IsValid)
            {
                sysBpmsEmailAccount emailAccount = new sysBpmsEmailAccount();
                ResultOperation resultOperation = emailAccount.Update((int)sysBpmsEmailAccount.e_ObjectTypeLU.Systemic, null, EmailAccountDTO.SMTP, EmailAccountDTO.Port,
                   EmailAccountDTO.MailPassword, EmailAccountDTO.Email);

                if (!resultOperation.IsSuccess)
                    return new PostMethodMessage(resultOperation.GetErrors(), DisplayMessageType.error);

                emailAccount.ID = EmailAccountDTO.ID;
                using (EmailAccountService EmailAccountService = new EmailAccountService())
                {
                    if (emailAccount.ID != Guid.Empty)
                        resultOperation = EmailAccountService.Update(emailAccount);
                    else
                        resultOperation = EmailAccountService.Add(emailAccount);
                }

                if (resultOperation.IsSuccess)
                    return new PostMethodMessage(SharedLang.Get("Success.Text"), DisplayMessageType.success);
                else
                    return new PostMethodMessage(resultOperation.GetErrors(), DisplayMessageType.error);
            }
            else
                return new PostMethodMessage(SharedLang.Get("NotFound.Text"), DisplayMessageType.error);
        }

        [HttpPost]
        public object TestEmail(EmailAccountDTO EmailAccountDTO)
        {
            if (ModelState.IsValid)
            {
                ResultOperation resultOperation = new EmailService().SendEmail(EmailAccountDTO.Email, EmailAccountDTO.MailPassword, EmailAccountDTO.SMTP, EmailAccountDTO.Port.ToIntObj(), new List<string>() { EmailAccountDTO.Email }, "", "", "تست ایمیل", "فراید تست ایمیل");
                if (resultOperation.IsSuccess)
                    return new PostMethodMessage(SharedLang.Get("Success.Text"), DisplayMessageType.success);
                else
                    return new PostMethodMessage("Test failed.", DisplayMessageType.error);
            }
            else
                return new PostMethodMessage(SharedLang.Get("NotFound.Text"), DisplayMessageType.error);
        }

        [HttpDelete]
        public object Delete(Guid ID)
        {
            if (ID != Guid.Empty)
            {
                using (EmailAccountService emailAccountService = new EmailAccountService())
                {
                    ResultOperation resultOperation = emailAccountService.Delete(ID);
                    if (resultOperation.IsSuccess)
                        return new PostMethodMessage(SharedLang.Get("Success.Text"), DisplayMessageType.success);
                    else
                        return new PostMethodMessage(resultOperation.GetErrors(), DisplayMessageType.error);
                }
            }
            return new PostMethodMessage(SharedLang.Get("NotFound.Text"), DisplayMessageType.error);
        }
    }
}