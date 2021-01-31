using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using DynamicBusiness.BPMS.Domain;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class ApplicationPageRepository : IApplicationPageRepository
    {
        private Db_BPMSEntities Context;
        public ApplicationPageRepository(Db_BPMSEntities context)
        {
            this.Context = context;
        }

        public void Add(sysBpmsApplicationPage applicationPage)
        {
            applicationPage.ID = Guid.NewGuid();
            this.Context.sysBpmsApplicationPages.Add(applicationPage);
        }

        public void Update(sysBpmsApplicationPage applicationPage)
        {
            Domain.sysBpmsApplicationPage retVal = (from p in this.Context.sysBpmsApplicationPages
                                                where p.ID == applicationPage.ID
                                             select p).FirstOrDefault();
            retVal.Load(applicationPage);
        }

        public void Delete(Guid applicationPageId)
        {
            Domain.sysBpmsApplicationPage ApplicationPage = this.Context.sysBpmsApplicationPages.FirstOrDefault(d => d.ID == applicationPageId);
            if (ApplicationPage != null)
            {
                this.Context.sysBpmsApplicationPages.Remove(ApplicationPage);
            }
        }

        public sysBpmsApplicationPage GetInfo(Guid ID)
        {
            return (from P in this.Context.sysBpmsApplicationPages
                    where P.ID == ID
                    select P).AsNoTracking().FirstOrDefault();
        }

        public List<sysBpmsApplicationPage> GetList(Guid? dynamicFormID, int? groupLU)
        {
            return this.Context.sysBpmsApplicationPages.Where(d =>
                 (!dynamicFormID.HasValue || d.DynamicForms.FirstOrDefault().ID == dynamicFormID) &&
                 (!groupLU.HasValue || d.GroupLU == groupLU)).OrderBy(c => c.ID).AsNoTracking().ToList();
        }

        public List<sysBpmsApplicationPage> GetAvailable(Guid? userID, bool? ShowInMenu)
        {
            List<sysBpmsApplicationPage> retVal = null;
            retVal = (from P in this.Context.sysBpmsApplicationPages
                      join A in this.Context.sysBpmsApplicationPageAccesses.Where(c => c.AllowView) on P.ID equals A.ApplicationPageID into access
                      join D in this.Context.sysBpmsDepartmentMembers on userID equals D.UserID into Dlist
                      where
                      (!ShowInMenu.HasValue || P.ShowInMenu == ShowInMenu) &&
                      (access.Count() == 0 ||
                      (
                      userID.HasValue &&
                      (access.Count(c => c.UserID.HasValue && c.UserID == userID) > 0 ||
                      access.Count(c => c.RoleLU.HasValue && Dlist.Count(d => c.RoleLU == d.RoleLU && (!c.DepartmentID.HasValue || c.DepartmentID == d.DepartmentID)) > 0) > 0)
                      ))
                      select P).OrderBy(c => c.DynamicForms.FirstOrDefault().Name).Include(c => c.DynamicForms).AsNoTracking().ToList();

            return retVal;
        }

    }
}
