using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicBusiness.BPMS.Domain;
using System.Data.Entity;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class ApplicationPageAccessRepository : IApplicationPageAccessRepository
    {
        private Db_BPMSEntities Context;

        public ApplicationPageAccessRepository(Db_BPMSEntities context)
        {
            this.Context = context;
        }

        public void Add(sysBpmsApplicationPageAccess applicationPageAccess)
        {
            applicationPageAccess.ID = Guid.NewGuid();
            this.Context.sysBpmsApplicationPageAccesses.Add(applicationPageAccess);
        }

        public void Delete(Guid id)
        {
            sysBpmsApplicationPageAccess applicationPageAccess = this.Context.sysBpmsApplicationPageAccesses.FirstOrDefault(d => d.ID == id);
            if (applicationPageAccess != null)
            {
                this.Context.sysBpmsApplicationPageAccesses.Remove(applicationPageAccess);
            }
        }

        public sysBpmsApplicationPageAccess GetInfo(Guid id)
        {
            return (from P in this.Context.sysBpmsApplicationPageAccesses
                    where P.ID == id
                    select P).AsNoTracking().FirstOrDefault();
        }
        public List<sysBpmsApplicationPageAccess> GetList(Guid departmentId)
        {
            return this.Context.sysBpmsApplicationPageAccesses.Where(d => d.DepartmentID == departmentId).AsNoTracking().ToList();
        }
        public List<sysBpmsApplicationPageAccess> GetList(Guid? applicationPageID, PagingProperties currentPaging)
        {
            using (Db_BPMSEntities db = new Db_BPMSEntities())
            {
                if (currentPaging != null)
                {
                    currentPaging.RowsCount = db.sysBpmsApplicationPageAccesses.Where(d => !applicationPageID.HasValue || d.ApplicationPageID == applicationPageID).Count();
                    if (Math.Ceiling(Convert.ToDecimal(currentPaging.RowsCount) / Convert.ToDecimal(currentPaging.PageSize)) < currentPaging.PageIndex)
                        currentPaging.PageIndex = 1;

                    return db.sysBpmsApplicationPageAccesses.Where(d => !applicationPageID.HasValue || d.ApplicationPageID == applicationPageID).OrderByDescending(p => p.ID).Skip((currentPaging.PageIndex - 1) * currentPaging.PageSize).Take(currentPaging.PageSize).AsNoTracking().ToList();
                }
                else
                    return db.sysBpmsApplicationPageAccesses.Where(d => !applicationPageID.HasValue || d.ApplicationPageID == applicationPageID).AsNoTracking().ToList();
            }
        }

        public void Update(sysBpmsApplicationPageAccess applicationPageAccess)
        {
            this.Context.Entry(applicationPageAccess).State = EntityState.Modified;
        }

        public bool GetUserAccess(Guid? userID, Guid applicationPageID, ElementBase.e_AccessType e_AccessType)
        {
            List<sysBpmsApplicationPageAccess> retVal = null;
            retVal = (from P in this.Context.sysBpmsApplicationPageAccesses
                      where
                      P.ApplicationPageID == applicationPageID &&
                      ((P.AllowAdd && ElementBase.e_AccessType.AllowAdd == e_AccessType) ||
                      (P.AllowEdit && ElementBase.e_AccessType.AllowEdit == e_AccessType) ||
                      (P.AllowDelete && ElementBase.e_AccessType.AllowDelete == e_AccessType) ||
                      (P.AllowView && ElementBase.e_AccessType.AllowView == e_AccessType))
                      select P).AsNoTracking().ToList();
            if (retVal.Count == 0)
                return true;
            if (!userID.HasValue)
                return false;
            if (retVal.Any(c => c.UserID == userID))
                return true;

            return this.Context.sysBpmsDepartmentMembers.Where(c => c.UserID == userID).ToList().Any(c =>
            retVal.Any(d =>
            (!d.DepartmentID.HasValue || d.DepartmentID == c.DepartmentID) &&
            (d.RoleLU == c.RoleLU)));
        }
    }
}
