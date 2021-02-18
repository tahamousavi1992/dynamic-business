using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using DynamicBusiness.BPMS.Domain;
namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class ElementRepository : IElementRepository
    {
        private Db_BPMSEntities Context;
        public ElementRepository(Db_BPMSEntities context)
        {
            this.Context = context;
        }

        public void Add(sysBpmsElement element)
        {
            this.Context.sysBpmsElements.Add(element);
        }

        public void Update(sysBpmsElement element)
        {
            this.Context.Entry(element).State = EntityState.Modified;
        }

        public void Delete(string elementId, Guid processId)
        {
            sysBpmsElement Element = this.Context.sysBpmsElements.FirstOrDefault(d => d.ID == elementId && d.ProcessID == processId);
            if (Element != null)
            {
                this.Context.sysBpmsElements.Remove(Element);
            }
        }

        public sysBpmsElement GetInfo(string ID, Guid processId)
        {
            return (from P in this.Context.sysBpmsElements
                    where P.ID == ID && P.ProcessID == processId
                    select P).AsNoTracking().FirstOrDefault();
        }

        public List<sysBpmsElement> GetList(Guid processID, int? TypeLU, string Name)
        {
            return this.Context.sysBpmsElements.Where(d =>
                (d.ProcessID == processID) &&
                (Name == string.Empty || d.Name == Name) &&
                (!TypeLU.HasValue || d.TypeLU == TypeLU)).AsNoTracking().ToList();
        }

        public List<sysBpmsElement> GetList(string[] elementID, Guid processId)
        {
            return this.Context.sysBpmsElements.Where(c => elementID.Contains(c.ID) && c.ProcessID == processId).AsNoTracking().ToList();
        }
    }
}
