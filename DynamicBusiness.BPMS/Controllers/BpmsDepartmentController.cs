using DotNetNuke.Web.Mvc.Framework.Controllers;
using DotNetNuke.Web.Mvc.Routing;
using DynamicBusiness.BPMS.BusinessLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using DynamicBusiness.BPMS.Domain;
using System.Web.Script.Serialization;
using DynamicBusiness.BPMS.SharedPresentation;

namespace DynamicBusiness.BPMS.Controllers
{
    public class BpmsDepartmentController : BpmsAdminApiControlBase
    {
        public BpmsDepartmentController()
        {

        }
        [HttpGet]
        public object GetList(Guid? SelectedID = null)
        {
            using (DepartmentService departmentService = new DepartmentService())
                return Json(new
                {
                    SelectedID = SelectedID,
                    DepartmentList = departmentService.HelperGetTreeList(SelectedID)
                });
        }

        [HttpGet]
        public object GetAddEdit(Guid? ID = null, Guid? ParentID = null)
        {
            if (ParentID.HasValue)
            {
                return new DepartmentDTO() { DepartmentID = ParentID };
            }
            else
            {
                using (EmailAccountService emailAccountService = new EmailAccountService())
                {
                    using (DepartmentService departmentService = new DepartmentService())
                    {
                        sysBpmsEmailAccount emailAccount = !ID.HasValue || ID == Guid.Empty ? null :
                            emailAccountService.GetList((int)sysBpmsEmailAccount.e_ObjectTypeLU.Department, ID, null).LastOrDefault();
                        return (new DepartmentDTO(ID.ToGuidObj() != Guid.Empty ? departmentService.GetInfo(ID.Value) : null, emailAccount));
                    }
                }
            }
        }

        [HttpPost]
        public object PostAddEdit(DepartmentDTO departmentDTO)
        {
            using (DepartmentService departmentService = new DepartmentService())
            {
                sysBpmsDepartment department = departmentDTO.ID != Guid.Empty ? departmentService.GetInfo(departmentDTO.ID) : new sysBpmsDepartment();
                department.Update(departmentDTO.DepartmentID, departmentDTO.Name);

                sysBpmsEmailAccount emailAccount = new sysBpmsEmailAccount();

                ResultOperation resultOperation = emailAccount.Update((int)sysBpmsEmailAccount.e_ObjectTypeLU.Department, department.ID, departmentDTO.SMTP, departmentDTO.Port, departmentDTO.MailPassword, departmentDTO.WorkEmail);
                if (!resultOperation.IsSuccess)
                    return new PostMethodMessage(resultOperation.GetErrors(), DisplayMessageType.error);
                if (departmentDTO.ID != Guid.Empty)
                    resultOperation = departmentService.Update(department, emailAccount);
                else
                    resultOperation = departmentService.Add(department, emailAccount);

                if (resultOperation.IsSuccess)
                    return new PostMethodMessage(SharedLang.Get("Success.Text"), DisplayMessageType.success);
                else
                    return new PostMethodMessage(resultOperation.GetErrors(), DisplayMessageType.error);
            }
        }

        [HttpDelete]
        public object Delete(Guid ID)
        {
            using (DepartmentService departmentService = new DepartmentService())
            {
                sysBpmsDepartment department = departmentService.GetInfo(ID);
                department.IsActive = false;
                ResultOperation resultOperation = departmentService.Update(department, null);

                if (resultOperation.IsSuccess)
                    return new PostMethodMessage(SharedLang.Get("Success.Text"), DisplayMessageType.success);
                else
                    return new PostMethodMessage(resultOperation.GetErrors(), DisplayMessageType.error);
            }
        }
    }
}