using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using DynamicBusiness.BPMS.Domain;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class APIAccessRepository : IAPIAccessRepository
    {
        private Db_BPMSEntities Context;
        public APIAccessRepository(Db_BPMSEntities context)
        {
            this.Context = context;
        }

        public void Update(sysBpmsAPIAccess apiAccess)
        {
            this.Context.Entry(apiAccess).State = EntityState.Modified;
        }

        public void Add(sysBpmsAPIAccess APIAccess)
        {
            APIAccess.ID = Guid.NewGuid();
            this.Context.sysBpmsAPIAccesses.Add(APIAccess);
        }

        public sysBpmsAPIAccess GetInfo(Guid ID)
        {
            return (from P in this.Context.sysBpmsAPIAccesses
                    where P.ID == ID
                    select P).AsNoTracking().FirstOrDefault();
        }

        public void Delete(Guid id)
        {
            sysBpmsAPIAccess sysBpmsAPIAccess = this.Context.sysBpmsAPIAccesses.FirstOrDefault(d => d.ID == id);
            if (sysBpmsAPIAccess != null)
            {
                this.Context.sysBpmsAPIAccesses.Remove(sysBpmsAPIAccess);
            }
        }

        public bool HasAccess(string ipAddress, string accessKey)
        {
            ipAddress = ipAddress.ToStringObj();
            accessKey = accessKey.ToStringObj();
            return (from P in this.Context.sysBpmsAPIAccesses
                    where
                    (P.IPAddress == string.Empty || P.IPAddress == null || P.IPAddress == ipAddress) &&
                    (P.AccessKey == accessKey)
                    select P).AsNoTracking().Count() > 0;
        }

        public List<sysBpmsAPIAccess> GetList(string Name, string IPAddress, string AccessKey, bool? IsActive, PagingProperties currentPaging)
        {
            List<sysBpmsAPIAccess> retVal = null;
            Name = Name.ToStringObj().Trim().ToLower();
            IPAddress = IPAddress.ToStringObj();
            AccessKey = AccessKey.ToStringObj();

            var query = (from P in this.Context.sysBpmsAPIAccesses
                         where
                         (IPAddress == string.Empty || P.IPAddress == IPAddress) &&
                         (AccessKey == string.Empty || P.AccessKey == AccessKey) &&
                         (Name == string.Empty || P.Name.Trim().ToLower().Contains(Name)) &&
                         (!IsActive.HasValue || P.IsActive == IsActive)
                         select P);
            if (currentPaging != null)
            {
                currentPaging.RowsCount = query.Count();

                if (Math.Ceiling(Convert.ToDecimal(currentPaging.RowsCount) / Convert.ToDecimal(currentPaging.PageSize)) < currentPaging.PageIndex)
                    currentPaging.PageIndex = 1;

                retVal = query.OrderBy(p => p.Name).Skip((currentPaging.PageIndex - 1) * currentPaging.PageSize).Take(currentPaging.PageSize).AsNoTracking().ToList();
            }
            else
                retVal = query.OrderBy(p => p.Name).AsNoTracking().ToList();
            return retVal;
        }

    }
}
