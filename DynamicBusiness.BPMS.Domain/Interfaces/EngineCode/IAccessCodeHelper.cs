using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    public interface IAccessCodeHelper
    {
        Guid? GetUserID(Guid departmentID, int roleCode);

        sysBpmsUser GetUser(Guid departmentID, int roleCode);

        List<sysBpmsUser> GetUserList(Guid departmentID, int? roleCode);

        List<sysBpmsDepartment> GetDepartmentList(Guid userID);

        int? GetRoleCode(Guid userID, Guid departmentID);

        string GetRoleCodeList(Guid userID, Guid? departmentID);

        bool AddRoleToUser(Guid userID, Guid departmentID, int roleCode);

        bool RemoveRoleFromUser(Guid userID, Guid departmentID, int roleCode);

        Guid? GetDepartmentHierarchyByUserId(Guid userID, int roleCode, bool goUpDepartment = true);
    }
}
