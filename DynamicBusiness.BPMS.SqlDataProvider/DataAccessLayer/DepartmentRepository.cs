using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicBusiness.BPMS.Domain;
using System.Data.Entity;
namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private Db_BPMSEntities Context;
        public DepartmentRepository(Db_BPMSEntities context)
        {
            this.Context = context;
        }

        public void Add(sysBpmsDepartment Department)
        {
            Department.ID = Guid.NewGuid();
            this.Context.sysBpmsDepartments.Add(Department);
        }

        public void Update(sysBpmsDepartment Department)
        {
            sysBpmsDepartment retVal = (from p in this.Context.sysBpmsDepartments
                                    where p.ID == Department.ID
                                 select p).FirstOrDefault();
            retVal.Load(Department);
        }

        public void Delete(Guid DepartmentId)
        {
            sysBpmsDepartment Department = this.Context.sysBpmsDepartments.FirstOrDefault(d => d.ID == DepartmentId);
            if (Department != null)
            {
                this.Context.sysBpmsDepartments.Remove(Department);
            }
        }

        public sysBpmsDepartment GetInfo(Guid ID)
        {
            return (from P in this.Context.sysBpmsDepartments
                    where P.ID == ID
                    select P).AsNoTracking().FirstOrDefault();
        }

        public List<sysBpmsDepartment> GetList(bool? IsActive, string Name, Guid? ParentDepartmentID)
        {
            List<sysBpmsDepartment> rettVal = this.Context.sysBpmsDepartments.Where(d =>
            (!IsActive.HasValue || d.IsActive == IsActive) &&
            (Name == string.Empty || d.Name.Contains(Name)) &&
            (!ParentDepartmentID.HasValue || d.DepartmentID == ParentDepartmentID)).OrderBy(c => c.Name).AsNoTracking().ToList();

            return rettVal;
        }

    }
}
