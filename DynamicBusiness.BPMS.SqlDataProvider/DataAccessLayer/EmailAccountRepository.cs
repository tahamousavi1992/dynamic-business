using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using DynamicBusiness.BPMS.Domain;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class EmailAccountRepository : IEmailAccountRepository
    {
        private Db_BPMSEntities Context;
        public EmailAccountRepository(Db_BPMSEntities context)
        {
            this.Context = context;
        }

        public void Add(sysBpmsEmailAccount emailAccount)
        {
            emailAccount.ID = Guid.NewGuid();
            this.Context.sysBpmsEmailAccounts.Add(emailAccount);
        }

        public void Update(sysBpmsEmailAccount emailAccount)
        {
            sysBpmsEmailAccount retVal = (from p in this.Context.sysBpmsEmailAccounts
                                          where p.ID == emailAccount.ID
                                          select p).FirstOrDefault();
            retVal.Load(emailAccount);
        }

        public void Delete(Guid emailAccountId)
        {
            sysBpmsEmailAccount emailAccount = this.Context.sysBpmsEmailAccounts.FirstOrDefault(d => d.ID == emailAccountId);
            if (emailAccount != null)
            {
                this.Context.sysBpmsEmailAccounts.Remove(emailAccount);
            }
        }

        public sysBpmsEmailAccount GetInfo(Guid ID)
        {
            return (from P in this.Context.sysBpmsEmailAccounts
                    where P.ID == ID
                    select P).AsNoTracking().FirstOrDefault();

        }

        public List<sysBpmsEmailAccount> GetList(int? ObjectTypeLU, Guid? ObjectID, PagingProperties currentPaging)
        {
            List<sysBpmsEmailAccount> rettVal = null;
            var query = this.Context.sysBpmsEmailAccounts.Where(d =>
              (!ObjectTypeLU.HasValue || d.ObjectTypeLU == ObjectTypeLU) &&
              (!ObjectID.HasValue || d.ObjectID == ObjectID)).AsNoTracking();
            if (currentPaging != null)
            {
                currentPaging.RowsCount = query.Count();
                if (Math.Ceiling(Convert.ToDecimal(currentPaging.RowsCount) / Convert.ToDecimal(currentPaging.PageSize)) < currentPaging.PageIndex)
                    currentPaging.PageIndex = 1;

                rettVal = query.OrderByDescending(p => p.Email).Skip((currentPaging.PageIndex - 1) * currentPaging.PageSize).Take(currentPaging.PageSize).AsNoTracking().ToList();
            }
            else rettVal = query.OrderByDescending(p => p.Email).ToList();

            return rettVal;
        }
    }
}
