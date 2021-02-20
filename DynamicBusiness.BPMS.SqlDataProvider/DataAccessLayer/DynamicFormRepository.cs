using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicBusiness.BPMS.Domain;
using System.Data.Entity;
namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class DynamicFormRepository : IDynamicFormRepository
    {
        private Db_BPMSEntities Context { get; set; }
        public DynamicFormRepository(Db_BPMSEntities context)
        {
            this.Context = context;
        }

        public void Add(sysBpmsDynamicForm dynamicForm)
        {
            dynamicForm.ID = Guid.NewGuid();
            this.Context.sysBpmsDynamicForms.Add(dynamicForm);
        }

        public void Update(sysBpmsDynamicForm dynamicForm)
        { 
            this.Context.Entry(dynamicForm.Clone()).State = EntityState.Modified;
        }

        public void Delete(Guid dynamicFormId)
        {
            sysBpmsDynamicForm dynamicForm = this.Context.sysBpmsDynamicForms.FirstOrDefault(d => d.ID == dynamicFormId);
            if (dynamicForm != null)
            {
                this.Context.sysBpmsDynamicForms.Remove(dynamicForm);
            }
        }

        public sysBpmsDynamicForm GetInfo(Guid ID)
        {
            return (from P in this.Context.sysBpmsDynamicForms
                    where P.ID == ID
                    select P).AsNoTracking().FirstOrDefault();
        }

        public sysBpmsDynamicForm GetInfoByPageID(Guid ApplicationPageID)
        {
            return (from P in this.Context.sysBpmsDynamicForms
                    where P.ApplicationPageID == ApplicationPageID
                    select P).AsNoTracking().FirstOrDefault();
        }

        public sysBpmsDynamicForm GetInfoByStepID(Guid StepID)
        {
            return (from P in this.Context.sysBpmsSteps
                    where P.ID == StepID
                    select P.DynamicForm).AsNoTracking().FirstOrDefault();
        }

        public List<sysBpmsDynamicForm> GetList(Guid? processId, Guid? applicationPageID, bool? getPages, string Name, bool? showInOverview, PagingProperties currentPaging)
        {
            Name = Name.ToStringObj().Trim();
            var query = this.Context.sysBpmsDynamicForms.AsNoTracking().Where(d =>
              (!showInOverview.HasValue || d.ShowInOverview == showInOverview) &&
              (!getPages.HasValue || (getPages == true && d.ApplicationPageID.HasValue) || (getPages == false && d.ProcessId.HasValue)) &&
              (!processId.HasValue || d.ProcessId == processId) &&
              (Name == string.Empty || d.Name.Contains(Name)) &&
              (!applicationPageID.HasValue || d.ApplicationPageID == applicationPageID)
            );
            if (currentPaging != null)
            {
                currentPaging.RowsCount = query.Count();
                if (Math.Ceiling(Convert.ToDecimal(currentPaging.RowsCount) / Convert.ToDecimal(currentPaging.PageSize)) < currentPaging.PageIndex)
                    currentPaging.PageIndex = 1;

                return query.OrderByDescending(p => p.CreatedDate).ThenBy(c => c.Name).Skip((currentPaging.PageIndex - 1) * currentPaging.PageSize).Take(currentPaging.PageSize).AsNoTracking().ToList();
            }
            else return query.OrderByDescending(p => p.CreatedDate).ToList();
        }
    }
}
