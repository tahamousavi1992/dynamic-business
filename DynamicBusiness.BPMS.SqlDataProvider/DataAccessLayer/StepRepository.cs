using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using DynamicBusiness.BPMS.Domain;
namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class StepRepository : IStepRepository
    {
        private Db_BPMSEntities Context;
        public StepRepository(Db_BPMSEntities context)
        {
            this.Context = context;
        }

        public void Add(sysBpmsStep Step)
        {
            Step.ID = Guid.NewGuid();
            this.Context.sysBpmsSteps.Add(Step);
        }

        public void Update(sysBpmsStep Step)
        {
            sysBpmsStep retVal = (from p in this.Context.sysBpmsSteps
                              where p.ID == Step.ID
                           select p).FirstOrDefault();
            retVal.Load(Step);
        }

        public void Delete(Guid StepId)
        {
            sysBpmsStep Step = this.Context.sysBpmsSteps.FirstOrDefault(d => d.ID == StepId);
            if (Step != null)
                this.Context.sysBpmsSteps.Remove(Step);
        }

        public sysBpmsStep GetInfo(Guid ID, string[] Includes)
        {
            return (from P in this.Context.sysBpmsSteps
                    where P.ID == ID
                    select P).Include(Includes).AsNoTracking().FirstOrDefault();
        }

        public List<sysBpmsStep> GetList(Guid? taskID, Guid? DynamicFormID)
        {
            return this.Context.sysBpmsSteps.Where(d =>
                (!taskID.HasValue || d.TaskID == taskID) &&
                (!DynamicFormID.HasValue || d.DynamicFormID == DynamicFormID)).OrderBy(c => c.Position).Include(c => c.sysBpmsDynamicForm).AsNoTracking().ToList();
        }
    }
}
