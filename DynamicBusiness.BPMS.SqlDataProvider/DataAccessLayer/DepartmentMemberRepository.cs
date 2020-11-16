using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicBusiness.BPMS.Domain;
using System.Data.Entity;
namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class DepartmentMemberRepository : IDepartmentMemberRepository
    {
        private Db_BPMSEntities Context;
        public DepartmentMemberRepository(Db_BPMSEntities context)
        {
            this.Context = context;
        }
        public void Add(sysBpmsDepartmentMember DepartmentMember)
        {
            DepartmentMember.ID = Guid.NewGuid();
            this.Context.sysBpmsDepartmentMembers.Add(DepartmentMember);
        }

        public void Update(sysBpmsDepartmentMember DepartmentMember)
        {
            sysBpmsDepartmentMember retVal = (from p in this.Context.sysBpmsDepartmentMembers
                                          where p.ID == DepartmentMember.ID
                                          select p).FirstOrDefault();
            retVal.Load(DepartmentMember);
        }

        public void Delete(Guid DepartmentMemberId)
        {
            sysBpmsDepartmentMember DepartmentMember = this.Context.sysBpmsDepartmentMembers.FirstOrDefault(d => d.ID == DepartmentMemberId);
            if (DepartmentMember != null)
            {
                this.Context.sysBpmsDepartmentMembers.Remove(DepartmentMember);
            }
        }

        public sysBpmsDepartmentMember GetInfo(Guid ID)
        {
            return (from P in this.Context.sysBpmsDepartmentMembers
                    where P.ID == ID
                    select P).AsNoTracking().FirstOrDefault();
        }

        public List<sysBpmsDepartmentMember> GetList(Guid? departmentID, int? roleLU, Guid? userID)
        {
            return this.Context.sysBpmsDepartmentMembers.Where(d =>
            (!departmentID.HasValue || d.DepartmentID == departmentID) &&
            (!roleLU.HasValue || d.RoleLU == roleLU) &&
            (!userID.HasValue || d.UserID == userID)).AsNoTracking().ToList();
        }

        public List<sysBpmsDepartmentMember> GetList(Guid? departmentID, int? roleLU, Guid? userID, PagingProperties currentPaging)
        {
            var query = this.Context.sysBpmsDepartmentMembers.Where(d =>
              (!departmentID.HasValue || d.DepartmentID == departmentID) &&
              (!roleLU.HasValue || d.RoleLU == roleLU) &&
              (!userID.HasValue || d.UserID == userID)).GroupBy(c => new { c.DepartmentID, c.UserID }).Select(c => new
              {
                  DepartmentID = c.Key.DepartmentID,
                  UserID = c.Key.UserID,
                  RoleLU = c.FirstOrDefault().RoleLU,
                  ID = c.FirstOrDefault().ID,
                  Department = c.FirstOrDefault().sysBpmsDepartment,
                  User = c.FirstOrDefault().sysBpmsUser,
              });

            if (currentPaging != null)
            {
                currentPaging.RowsCount = query.Count();
                if (Math.Ceiling(Convert.ToDecimal(currentPaging.RowsCount) / Convert.ToDecimal(currentPaging.PageSize)) < currentPaging.PageIndex)
                    currentPaging.PageIndex = 1;

                return query.OrderByDescending(p => p.ID).Skip((currentPaging.PageIndex - 1) * currentPaging.PageSize).Take(currentPaging.PageSize).AsNoTracking().ToList().Select(c => new sysBpmsDepartmentMember()
                {
                    DepartmentID = c.DepartmentID,
                    UserID = c.UserID,
                    RoleLU = c.RoleLU,
                    ID = c.ID,
                    sysBpmsDepartment = c.Department,
                    sysBpmsUser = c.User,
                }).ToList();
            }
            else return query.OrderByDescending(p => p.ID).ToList().Select(c => new sysBpmsDepartmentMember()
            {
                DepartmentID = c.DepartmentID,
                UserID = c.UserID,
                RoleLU = c.RoleLU,
                ID = c.ID,
                sysBpmsDepartment = c.Department,
                sysBpmsUser = c.User,
            }).ToList();
        }

        public List<sysBpmsDepartmentMember> GetList(Guid[] listDepartmentID, int? roleLU)
        {
            bool AllDepartment = listDepartmentID == null;
            listDepartmentID = listDepartmentID ?? new Guid[] { };
            return this.Context.sysBpmsDepartmentMembers.Where(d =>
            (AllDepartment || listDepartmentID.Contains(d.DepartmentID)) &&
            (!roleLU.HasValue || d.RoleLU == roleLU)).AsNoTracking().ToList();
        }

        public List<sysBpmsDepartmentMember> GetListHierarchy(Guid[] listDepartmentID, int roleLU, bool goUpDepartment)
        {
            listDepartmentID = listDepartmentID ?? new Guid[] { };
            List<Guid> listHierarchy = new List<Guid>();
            if (goUpDepartment)
            {
                foreach (var item in this.Context.sysBpmsDepartments.Where(c => listDepartmentID.Contains(c.ID)))
                {
                    this.GetListHierarchy(item, listHierarchy);
                }
            }
            else
            {
                listHierarchy = listDepartmentID.Distinct().ToList();
            }

            return this.Context.sysBpmsDepartmentMembers.Where(c => listHierarchy.Contains(c.DepartmentID) && c.RoleLU == roleLU).AsNoTracking().ToList();
        }

        private void GetListHierarchy(sysBpmsDepartment childDepartment, List<Guid> Hierarchy)
        {
            Hierarchy.Add(childDepartment.ID);
            foreach (var item in this.Context.sysBpmsDepartments.Where(c => c.ID == childDepartment.DepartmentID))
            {
                this.GetListHierarchy(item, Hierarchy);
            }
        }
    }
}
