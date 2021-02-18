using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using DynamicBusiness.BPMS.Domain;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class LaneRepository : ILaneRepository
    {
        private Db_BPMSEntities Context;
        public LaneRepository(Db_BPMSEntities context)
        {
            this.Context = context;
        }

        public void Add(sysBpmsLane lane)
        {
            this.Context.sysBpmsLanes.Add(lane);
        }

        public void Update(sysBpmsLane lane)
        {
            this.Context.Entry(lane).State = EntityState.Modified;
        }

        public void Delete(Guid id)
        {
            sysBpmsLane lane = this.Context.sysBpmsLanes.FirstOrDefault(d => d.ID == id);
            if (lane != null)
            {
                this.Context.sysBpmsLanes.Remove(lane);
            }
        }

        public sysBpmsLane GetInfo(Guid id)
        {
            return this.Context.sysBpmsLanes.AsNoTracking().FirstOrDefault(d => d.ID == id); ;
        }

        public List<sysBpmsLane> GetList(Guid? ProcessID)
        {
            return this.Context.sysBpmsLanes.Where(d =>
            (!ProcessID.HasValue || d.Element.ProcessID == ProcessID)).OrderBy(c => c.Element.Name).AsNoTracking().ToList();
        }
    }
}
