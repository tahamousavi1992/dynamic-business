using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    public interface IDepartmentRepository
    {
        void Add(sysBpmsDepartment Department);
        void Update(sysBpmsDepartment Department);
        void Delete(Guid DepartmentId);
        sysBpmsDepartment GetInfo(Guid ID);
        List<sysBpmsDepartment> GetList(bool? IsActive, string Name, Guid? ParentDepartmentID);
    }
}
