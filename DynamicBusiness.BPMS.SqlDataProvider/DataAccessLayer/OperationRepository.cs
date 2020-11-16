using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicBusiness.BPMS.Domain;
using System.Data.Entity;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class OperationRepository : IOperationRepository
    {
        private Db_BPMSEntities Context;

        public OperationRepository(Db_BPMSEntities context)
        {
            this.Context = context;
        }

        public void Add(sysBpmsOperation operation)
        {
            operation.ID = Guid.NewGuid();
            this.Context.sysBpmsOperations.Add(operation);
        }

        public void Delete(Guid id)
        {
            sysBpmsOperation operation = this.Context.sysBpmsOperations.FirstOrDefault(d => d.ID == id);
            if (operation != null)
            {
                this.Context.sysBpmsOperations.Remove(operation);
            }
        }

        public sysBpmsOperation GetInfo(Guid id)
        {
            return (from P in this.Context.sysBpmsOperations
                    where P.ID == id
                    select P).AsNoTracking().FirstOrDefault();
        }

        public List<sysBpmsOperation> GetList(int? GroupLU, PagingProperties currentPaging)
        {
            using (Db_BPMSEntities db = new Db_BPMSEntities())
            {
                if (currentPaging != null)
                {
                    currentPaging.RowsCount = db.sysBpmsOperations.Where(d => !GroupLU.HasValue || d.GroupLU == GroupLU).Count();
                    if (Math.Ceiling(Convert.ToDecimal(currentPaging.RowsCount) / Convert.ToDecimal(currentPaging.PageSize)) < currentPaging.PageIndex)
                        currentPaging.PageIndex = 1;

                    return db.sysBpmsOperations.Where(d => !GroupLU.HasValue || d.GroupLU == GroupLU).OrderBy(p => p.Name).Skip((currentPaging.PageIndex - 1) * currentPaging.PageSize).Take(currentPaging.PageSize).AsNoTracking().ToList();
                }
                else
                    return db.sysBpmsOperations.Where(d => !GroupLU.HasValue || d.GroupLU == GroupLU).OrderBy(p => p.Name).AsNoTracking().ToList();
            }
        }

        public void Update(sysBpmsOperation operation)
        {
            sysBpmsOperation retVal = (from p in this.Context.sysBpmsOperations
                                   where p.ID == operation.ID
                                select p).FirstOrDefault();
            retVal.Load(operation);
        }

    }
}
