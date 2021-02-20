using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicBusiness.BPMS.Domain;
using System.Data.Entity;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class EntityDefRepository : IEntityDefRepository
    {
        private Db_BPMSEntities Context;

        public EntityDefRepository(Db_BPMSEntities context)
        {
            this.Context = context;
        }

        public void Add(sysBpmsEntityDef entityDef)
        {
            entityDef.ID = Guid.NewGuid();
            this.Context.sysBpmsEntityDefs.Add(entityDef);
        }

        public void Update(sysBpmsEntityDef entityDef)
        {
            this.Context.Entry(entityDef.Clone()).State = EntityState.Modified;
        }

        public void Delete(Guid entityDefId)
        {
            sysBpmsEntityDef EntityDef = this.Context.sysBpmsEntityDefs.FirstOrDefault(d => d.ID == entityDefId);
            if (EntityDef != null)
            {
                this.Context.sysBpmsEntityDefs.Remove(EntityDef);
            }
        }

        public sysBpmsEntityDef GetInfo(Guid ID)
        {
            return (from P in this.Context.sysBpmsEntityDefs
                    where P.ID == ID
                    select P).AsNoTracking().FirstOrDefault();
        }
        public sysBpmsEntityDef GetInfo(string name)
        {
            return (from P in this.Context.sysBpmsEntityDefs
                    where P.Name == name
                    select P).AsNoTracking().FirstOrDefault();
        }

        public List<sysBpmsEntityDef> GetList(string name, bool? isActive, PagingProperties currentPaging)
        {
            using (Db_BPMSEntities db = new Db_BPMSEntities())
            {
                name = name ?? string.Empty;
                var query = db.sysBpmsEntityDefs.Where(d =>
                  (name == string.Empty || d.Name.Contains(name) || d.DisplayName.Contains(name)) &&
                  (!isActive.HasValue || d.IsActive == isActive));
                if (currentPaging != null)
                {
                    currentPaging.RowsCount = query.Count();
                    if (Math.Ceiling(Convert.ToDecimal(currentPaging.RowsCount) / Convert.ToDecimal(currentPaging.PageSize)) < currentPaging.PageIndex)
                        currentPaging.PageIndex = 1;

                    return query.OrderBy(p => p.Name).Skip((currentPaging.PageIndex - 1) * currentPaging.PageSize).Take(currentPaging.PageSize).AsNoTracking().ToList();
                }
                else return query.OrderBy(p => p.Name).AsNoTracking().ToList();

            }
        }

        public List<string> GetList(Guid relationToEntityId)
        {
            string entityID = relationToEntityId.ToStringObj();
            using (Db_BPMSEntities db = new Db_BPMSEntities())
            {
                return db.sysBpmsEntityDefs.Where(d => d.DesignXML.Contains(entityID)).OrderBy(p => p.Name)
                    .Select(c => c.Name).AsNoTracking().ToList();

            }
        }
    }
}
