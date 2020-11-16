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
using DotNetNuke.Entities.Users;

namespace DynamicBusiness.BPMS.Controllers
{ 
    public class BpmsUserController : BpmsAdminApiControlBase
    {
        // GET: BPMSUser
        [HttpGet]
        public object GetList([System.Web.Http.FromUri] UserIndexSearchDTO indexSearchVM)
        { 
            using (UserService userService = new UserService())
            {
                List<sysBpmsUser> list = userService.GetList(
                 (indexSearchVM.IsAdvSearch ? indexSearchVM.AdvName : indexSearchVM.Name),
                 (indexSearchVM.IsAdvSearch ? indexSearchVM.AdvRoleCode : null),
                 (indexSearchVM.IsAdvSearch ? indexSearchVM.AdvDepartmentID : null),
                 indexSearchVM.GetPagingProperties);
                using (LURowService lURowService = new LURowService())
                {
                    using (DepartmentService departmentService = new DepartmentService())
                        indexSearchVM.Update(
                         departmentService.GetList(true, "", null),
                         lURowService.GetList(sysBpmsLUTable.e_LUTable.DepartmentRoleLU.ToString()));
                }
                indexSearchVM.Update(list.Select(c => new UserDTO(c)).ToList());
                return indexSearchVM;
            }
        }

        [HttpGet]
        public object GetAddEdit(Guid ID)
        {
            sysBpmsEmailAccount emailAccount = ID == Guid.Empty ? null : new EmailAccountService().GetList((int)sysBpmsEmailAccount.e_ObjectTypeLU.User, ID, null).LastOrDefault();
            return (new UserDTO(new UserService().GetInfo(ID) ?? new sysBpmsUser(), emailAccount));
        }

        [HttpPost]
        public object PostAddEdit(UserDTO UserDTO)
        {
            sysBpmsUser user = new sysBpmsUser(UserDTO.ID, UserDTO.Username, UserDTO.FirstName, UserDTO.LastName,
                UserDTO.Email, UserDTO.Tel, UserDTO.Mobile);

            ResultOperation resultOperation = new ResultOperation();
            sysBpmsEmailAccount emailAccount = null;
            if (!string.IsNullOrWhiteSpace(UserDTO.EmailAccountDTO?.Email) ||
                !string.IsNullOrWhiteSpace(UserDTO.EmailAccountDTO?.SMTP) ||
                 !string.IsNullOrWhiteSpace(UserDTO.EmailAccountDTO?.MailUserName))
            {
                emailAccount = new sysBpmsEmailAccount();
                resultOperation = emailAccount.Update((int)sysBpmsEmailAccount.e_ObjectTypeLU.User, user.ID, UserDTO.EmailAccountDTO?.SMTP, UserDTO.EmailAccountDTO?.Port,
                  UserDTO.EmailAccountDTO?.MailUserName, UserDTO.EmailAccountDTO?.MailPassword, UserDTO.EmailAccountDTO?.Email);
                if (!resultOperation.IsSuccess)
                    return new PostMethodMessage(resultOperation.GetErrors(), DisplayMessageType.error);
            }
            if (!string.IsNullOrWhiteSpace(user.Username))
            {
                UserCodeHelper userCodeHelper = new UserCodeHelper(null, null);
                if (userCodeHelper.GetSiteUser(user.Username) == null)
                {
                    bool createResult = userCodeHelper.CreateSiteUser(user.Username, user.FirstName, user.LastName, user.Email, (string.IsNullOrWhiteSpace(UserDTO.Password) ? UserController.GeneratePassword() : UserDTO.Password), false);
                    if (!createResult)
                        return new PostMethodMessage(SharedLang.Get("CreateUserError.Text"), DisplayMessageType.error);
                }
            }

            using (UserService userService = new UserService())
            {
                if (user.ID != Guid.Empty)
                    resultOperation = userService.Update(user, emailAccount);
                else
                    resultOperation = userService.Add(user, emailAccount);
            }

            if (resultOperation.IsSuccess)
                return new PostMethodMessage(SharedLang.Get("Success.Text"), DisplayMessageType.success);
            else
                return new PostMethodMessage(resultOperation.GetErrors(), DisplayMessageType.error);
        }

        [HttpDelete]
        public object Delete(Guid ID)
        {
            ResultOperation resultOperation = new UserService().Delete(ID);
            if (resultOperation.IsSuccess)
                return new PostMethodMessage(SharedLang.Get("Success.Text"), DisplayMessageType.success);
            return new PostMethodMessage(resultOperation.GetErrors(), DisplayMessageType.error);
        }
    }
}