using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using DynamicBusiness.BPMS.Domain;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class LUTableRepository : ILUTableRepository
    {
        private Db_BPMSEntities Context;
        public LUTableRepository(Db_BPMSEntities context)
        {
            this.Context = context;
        }

        public List<sysBpmsLUTable> GetList()
        {
            return (from P in this.Context.sysBpmsLUTables
                    where
                    (P.IsSystemic == false) &&
                    (P.IsActive)
                    select P).AsNoTracking().ToList();
        }

        public sysBpmsLUTable GetInfo(Guid ID)
        {
            return (from P in this.Context.sysBpmsLUTables
                    where
                    (P.ID == ID)
                    select P).AsNoTracking().FirstOrDefault();
        }

    }
}
