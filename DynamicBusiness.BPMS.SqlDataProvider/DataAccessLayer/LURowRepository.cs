using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using DynamicBusiness.BPMS.Domain;
namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class LURowRepository : ILURowRepository
    {
        private Db_BPMSEntities Context;
        public LURowRepository(Db_BPMSEntities context)
        {
            this.Context = context;
        }

        public int MaxOrderByLUTableID(Guid LUTableID)
        {
            return DomainUtility.toInt((from p in this.Context.sysBpmsLURows
                                        where p.LUTableID == LUTableID
                                        select (int?)p.DisplayOrder).Max(c => c == null ? 0 : c));
        }
        public int MaxCodeOfByLUTableID(Guid LUTableID)
        {
            int retVal = 0;

            List<string> retValCodeofs = (from p in this.Context.sysBpmsLURows
                                          where p.LUTableID == LUTableID
                                          select p.CodeOf).AsNoTracking().ToList();
            if (retValCodeofs.Count > 0)
                retVal = DomainUtility.toInt(retValCodeofs.Max(c => c == null ? 0 : Convert.ToInt32(c)));

            return retVal;
        }
        public List<sysBpmsLURow> GetList(Guid LUTableID, string NameOf, bool? IsActive)
        {
            NameOf = NameOf.Trim().ToLower();

            return (from P in this.Context.sysBpmsLURows
                    where
                    (P.LUTableID == LUTableID) &&
                    (NameOf.Trim() == string.Empty || P.NameOf.Trim().ToLower().Contains(NameOf)) &&
                    (!IsActive.HasValue || P.IsActive == IsActive)
                    select P).OrderBy(c => c.DisplayOrder).AsNoTracking().ToList();
        }
        public List<sysBpmsLURow> GetList(Guid LUTableID, string NameOf, bool? IsActive, PagingProperties currentPaging)
        {
            List<sysBpmsLURow> retVal = null;
            NameOf = NameOf.Trim().ToLower();
            var query = (from P in this.Context.sysBpmsLURows
                         where
                         (P.LUTableID == LUTableID) &&
                         (NameOf.Trim() == string.Empty || P.NameOf.Trim().ToLower().Contains(NameOf)) &&
                         (!IsActive.HasValue || P.IsActive == IsActive)
                         select P);
            if (currentPaging != null)
            {
                currentPaging.RowsCount = query.Count();

                if (Math.Ceiling(Convert.ToDecimal(currentPaging.RowsCount) / Convert.ToDecimal(currentPaging.PageSize)) < currentPaging.PageIndex)
                    currentPaging.PageIndex = 1;

                retVal = query.OrderBy(c => c.DisplayOrder).Skip((currentPaging.PageIndex - 1) * currentPaging.PageSize).Take(currentPaging.PageSize).AsNoTracking().ToList();
            }
            else
                return query.AsNoTracking().ToList();

            return retVal;
        }

        public List<sysBpmsLURow> GetList(string Alias)
        {
            return (from P in this.Context.sysBpmsLURows
                    join F in this.Context.sysBpmsLUTables on P.LUTableID equals F.ID
                    where
                    (F.Alias == Alias) &&
                    (F.IsActive == P.IsActive == true)
                    select P).OrderBy(c => c.DisplayOrder).AsNoTracking().ToList();
        }

        public sysBpmsLURow GetInfo(Guid ID)
        {
            return (from P in this.Context.sysBpmsLURows
                    where P.ID == ID
                    select P).AsNoTracking().FirstOrDefault();
        }

        public sysBpmsLURow GetInfo(string Alias, string CodeOf)
        {
            return (from P in this.Context.sysBpmsLURows
                    join F in this.Context.sysBpmsLUTables on P.LUTableID equals F.ID
                    where
                    (P.CodeOf == CodeOf) &&
                    (F.Alias == Alias)
                    select P).AsNoTracking().FirstOrDefault();
        }

        public sysBpmsLURow GetInfoByName(string Alias, string NameOf)
        {
            return (from P in this.Context.sysBpmsLURows
                    join F in this.Context.sysBpmsLUTables on P.LUTableID equals F.ID
                    where
                    (P.NameOf == NameOf) &&
                    (F.Alias == Alias)
                    select P).AsNoTracking().FirstOrDefault();
        }

        public sysBpmsLURow GetInfo(Guid ID, Guid LUTableID, string CodeOf)
        {
            return (from P in this.Context.sysBpmsLURows
                    where
                    (P.ID != ID) &&
                    (P.LUTableID == LUTableID) &&
                    (P.CodeOf == CodeOf)
                    select P).AsNoTracking().FirstOrDefault();
        }

        public string GetNameOfByAlias(string Alias, string CodeOf)
        {
            string retVal = null;
            retVal = (from p in this.Context.sysBpmsLURows
                      join r in this.Context.sysBpmsLUTables
                          on p.LUTableID equals r.ID
                      where r.Alias.Trim() == Alias.Trim() && p.CodeOf == CodeOf
                      select p.NameOf).AsNoTracking().FirstOrDefault();
            return retVal != null ? retVal : "";
        }

        public void Update(sysBpmsLURow lurow)
        {
            this.Context.Entry(lurow).State = EntityState.Modified;
        }

        public void Delete(Guid processGroupId)
        {
            sysBpmsLURow sysBpmsLURow = this.Context.sysBpmsLURows.FirstOrDefault(d => d.ID == processGroupId);
            if (sysBpmsLURow != null)
            {
                this.Context.sysBpmsLURows.Remove(sysBpmsLURow);
            }
        }


        public void Add(sysBpmsLURow LURow)
        {
            LURow.ID = Guid.NewGuid();
            this.Context.sysBpmsLURows.Add(LURow);
        }
    }
}
