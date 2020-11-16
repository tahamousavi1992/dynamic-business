using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    public interface IDepartmentMemberRepository
    {
        void Add(sysBpmsDepartmentMember DepartmentMember);
        void Update(sysBpmsDepartmentMember DepartmentMember);
        void Delete(Guid DepartmentMemberId);
        sysBpmsDepartmentMember GetInfo(Guid ID);
        List<sysBpmsDepartmentMember> GetList(Guid? departmentID, int? roleLU, Guid? userID);
        List<sysBpmsDepartmentMember> GetList(Guid? departmentID, int? roleLU, Guid? userID, PagingProperties currentPaging);
        List<sysBpmsDepartmentMember> GetList(Guid[] listDepartmentID, int? roleLU);
        List<sysBpmsDepartmentMember> GetListHierarchy(Guid[] listDepartmentID, int roleLU, bool goUpDepartment);
    }
}
