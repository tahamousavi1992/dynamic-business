using DynamicBusiness.BPMS.BusinessLogic;
using DynamicBusiness.BPMS.Domain;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;
using DynamicBusiness.BPMS.SharedPresentation;
using System;

namespace DynamicBusiness.BPMS.Controllers
{
    public class BpmsDepartmentMemberController : BpmsAdminApiControlBase
    {
        public object GetList([System.Web.Http.FromUri]DepartmentMemberIndexSearchDTO indexSearchVM)
        { 
            using (DepartmentMemberService departmentMemberService = new DepartmentMemberService())
            {
                indexSearchVM.Update(departmentMemberService.GetList(indexSearchVM.DepartmentID, null, null, indexSearchVM.GetPagingProperties).Select(c => new DepartmentMemberListDTO(c)).ToList());
            }
            return indexSearchVM;
        }

        [HttpGet]
        public object GetAddEdit(Guid DepartmentID, Guid? UserID = null)
        {

            using (LURowService lURowService = new LURowService())
            {
                using (UserService userService = new UserService())
                {
                    DepartmentMemberAddEditDTO departmentMemberAddEditDTO = new DepartmentMemberAddEditDTO(DepartmentID, UserID);
                    departmentMemberAddEditDTO.ListUsers = userService.GetList("", null).Select(c => new UserDTO(c)).ToList();
                    departmentMemberAddEditDTO.ListRoles = lURowService.GetList("DepartmentRoleLU").Select(c => new LURowDTO(c)).ToList();
                    return departmentMemberAddEditDTO;
                }
            }


        }

        [HttpPost]
        public object PostAddEdit(DepartmentMemberAddEditDTO departmentMemberVM)
        {
            departmentMemberVM.Roles = base.MyRequest.Form["Roles"].ToStringObj().Split(',').Select(c => new QueryModel(c.ToString(), "")).ToList();
            ResultOperation resultOperation = new ResultOperation();
            using (DepartmentMemberService departmentMemberService = new DepartmentMemberService())
            {
                List<sysBpmsDepartmentMember> list = departmentMemberService.GetList(departmentMemberVM.DepartmentID, null, departmentMemberVM.UserID);
                foreach (var item in list.Where(c => !departmentMemberVM.Roles.Select(d => d.Key).Contains(c.RoleLU.ToString())))
                {
                    if (resultOperation.IsSuccess)
                        resultOperation = departmentMemberService.Delete(item.ID);
                }
                foreach (var item in departmentMemberVM.Roles.Where(c => !list.Any(d => d.RoleLU == c.Key.ToIntObj())))
                {
                    if (resultOperation.IsSuccess)
                    {
                        sysBpmsDepartmentMember newMember = new sysBpmsDepartmentMember();
                        newMember.Update(departmentMemberVM.DepartmentID, departmentMemberVM.UserID, item.Key.ToIntObj());
                        resultOperation = departmentMemberService.Add(newMember);
                    }
                }
            }
            if (resultOperation.IsSuccess)
                return new PostMethodMessage(SharedLang.Get("Success.Text"), DisplayMessageType.success);
            else
                return new PostMethodMessage(resultOperation.GetErrors(), DisplayMessageType.error);

        }

        [HttpDelete]
        public object Delete(Guid ID)
        {
            using (DepartmentMemberService departmentMemberService = new DepartmentMemberService())
            {
                ResultOperation resultOperation = departmentMemberService.Delete(ID);
                if (resultOperation.IsSuccess)
                    return new PostMethodMessage(SharedLang.Get("Success.Text"), DisplayMessageType.success);
                else
                    return new PostMethodMessage(resultOperation.GetErrors(), DisplayMessageType.error);
            }
        }
    }
}