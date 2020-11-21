using DynamicBusiness.BPMS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class AccessCodeHelper : IAccessCodeHelper
    {
        private IUnitOfWork UnitOfWork { get; set; }
        public AccessCodeHelper(IUnitOfWork unitOfWork)
        {
            this.UnitOfWork = unitOfWork;
        }

        public Guid? GetUserID(Guid departmentID, int roleCode)
        {
            return new DepartmentMemberService(this.UnitOfWork).GetList(departmentID, roleCode, null).FirstOrDefault()?.UserID;
        }

        public sysBpmsUser GetUser(Guid departmentID, int roleCode)
        {
            return new DepartmentMemberService(this.UnitOfWork).GetList(departmentID, roleCode, null).FirstOrDefault()?.sysBpmsUser;
        }

        public List<sysBpmsUser> GetUserList(Guid departmentID, int? roleCode)
        {
            return new DepartmentMemberService(this.UnitOfWork).GetList(departmentID, roleCode, null).Select(c => c.sysBpmsUser).ToList();
        }

        public List<sysBpmsDepartment> GetDepartmentList(Guid userID)
        {
            return new DepartmentMemberService(this.UnitOfWork).GetList(null, null, userID).Select(c => c.sysBpmsDepartment).ToList();
        }

        public int? GetRoleCode(Guid userID, Guid departmentID)
        {
            return new DepartmentMemberService(this.UnitOfWork).GetList(departmentID, null, userID).FirstOrDefault()?.RoleLU;
        }

        public string GetRoleCodeList(Guid userID, Guid? departmentID)
        {
            return string.Join(",", new DepartmentMemberService(this.UnitOfWork).GetList(departmentID, null, userID).Select(c => c.RoleLU).ToList());
        }

        public bool AddRoleToUser(Guid userID, Guid departmentID, int roleCode)
        {
            DepartmentMemberService departmentMemberService = new DepartmentMemberService(this.UnitOfWork);
            if (departmentMemberService.GetList(departmentID, roleCode, userID).Count == 0)
            {
                sysBpmsDepartmentMember sysBpmsDepartmentMember = new sysBpmsDepartmentMember();
                sysBpmsDepartmentMember.Update(departmentID, userID, roleCode);
                departmentMemberService.Add(sysBpmsDepartmentMember);
            }
            return true;
        }

        public bool RemoveRoleFromUser(Guid userID, Guid departmentID, int roleCode)
        {
            DepartmentMemberService departmentMemberService = new DepartmentMemberService(this.UnitOfWork);
            var data = departmentMemberService.GetList(departmentID, roleCode, userID).FirstOrDefault();
            if (data != null)
            {
                departmentMemberService.Delete(data.ID);
            }
            return true;
        }

        public Guid? GetDepartmentHierarchyByUserId(Guid userID, int roleCode, bool goUpDepartment = true)
        {
            Guid[] departments = new DepartmentMemberService(this.UnitOfWork).GetList(null, null, userID).Select(c => c.DepartmentID).Distinct().ToArray();
            return new DepartmentMemberService(this.UnitOfWork).GetListHierarchy(departments, roleCode, goUpDepartment).FirstOrDefault()?.DepartmentID;
        }

    }
}
