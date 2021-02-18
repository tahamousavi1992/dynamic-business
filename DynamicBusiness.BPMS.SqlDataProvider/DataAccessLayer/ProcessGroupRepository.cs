using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicBusiness.BPMS.Domain;
using System.Data.Entity;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class ProcessGroupRepository : IProcessGroupRepository
    {
        private Db_BPMSEntities Context;
        public ProcessGroupRepository(Db_BPMSEntities context)
        {
            this.Context = context;
        }

        public void Add(sysBpmsProcessGroup processGroup)
        {
            processGroup.ID = Guid.NewGuid();
            this.Context.sysBpmsProcessGroups.Add(processGroup);
        }

        public void Update(sysBpmsProcessGroup processGroup)
        {
            this.Context.Entry(processGroup).State = EntityState.Modified;
        }

        public void Delete(Guid processGroupId)
        {
            sysBpmsProcessGroup ProcessGroup = this.Context.sysBpmsProcessGroups.FirstOrDefault(d => d.ID == processGroupId);
            if (ProcessGroup != null)
            {
                this.Context.sysBpmsProcessGroups.Remove(ProcessGroup);
            }
        }

        public sysBpmsProcessGroup GetInfo(Guid id)
        {
            return (from P in this.Context.sysBpmsProcessGroups
                    where P.ID == id
                    select P).AsNoTracking().FirstOrDefault();
        }

        public List<sysBpmsProcessGroup> GetList(string name, Guid? parentProcessGroupID)
        {
            name = name.ToStringObj();
            List<sysBpmsProcessGroup> rettVal = this.Context.sysBpmsProcessGroups.Where(d =>
            (name == string.Empty || d.Name.Contains(name)) &&
            (!parentProcessGroupID.HasValue || d.ProcessGroupID == parentProcessGroupID)).OrderBy(c => c.Name).AsNoTracking().ToList();

            return rettVal;
        }

    }
}
