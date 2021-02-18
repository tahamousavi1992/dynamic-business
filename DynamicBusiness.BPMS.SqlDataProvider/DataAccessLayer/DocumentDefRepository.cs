using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using DynamicBusiness.BPMS.Domain;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class DocumentDefRepository : IDocumentDefRepository
    {
        private Db_BPMSEntities Context;
        public DocumentDefRepository(Db_BPMSEntities context)
        {
            this.Context = context;
        }

        public void Add(sysBpmsDocumentDef documentDef)
        {
            documentDef.ID = Guid.NewGuid();
            this.Context.sysBpmsDocumentDefs.Add(documentDef);
        }

        public void Update(sysBpmsDocumentDef documentDef)
        {
            this.Context.Entry(documentDef).State = EntityState.Modified;
        }

        public void Delete(Guid DocumentDefId)
        {
            sysBpmsDocumentDef documentDef = this.Context.sysBpmsDocumentDefs.FirstOrDefault(d => d.ID == DocumentDefId);
            if (documentDef != null)
            {
                this.Context.sysBpmsDocumentDefs.Remove(documentDef);
            }
        }

        public sysBpmsDocumentDef GetInfo(Guid ID)
        {
            return (from P in this.Context.sysBpmsDocumentDefs
                    where P.ID == ID
                    select P).AsNoTracking().FirstOrDefault();
        }

        public List<sysBpmsDocumentDef> GetList(Guid? DocumentFolderID, string NameOf, string DisplayName, bool? IsActive, bool? IsSystemic, PagingProperties currentPaging)
        {
            NameOf = DomainUtility.toString(NameOf);
            DisplayName = DomainUtility.toString(DisplayName);
            List<sysBpmsDocumentDef> retVal;
            var query = this.Context.sysBpmsDocumentDefs.Where(d =>
            (NameOf == string.Empty || d.NameOf.Contains(NameOf)) &&
            (DisplayName == string.Empty || d.DisplayName.Contains(DisplayName)) &&
            (!IsSystemic.HasValue || d.IsSystemic == IsSystemic) &&
            (!IsActive.HasValue || d.IsActive == IsActive) &&
            (!DocumentFolderID.HasValue || d.DocumentFolderID == DocumentFolderID));
           
            if (currentPaging != null)
            {
                currentPaging.RowsCount = query.Count();
                if (Math.Ceiling(Convert.ToDecimal(currentPaging.RowsCount) / Convert.ToDecimal(currentPaging.PageSize)) < currentPaging.PageIndex)
                    currentPaging.PageIndex = 1;

                retVal = query.OrderByDescending(p => p.DisplayName).Skip((currentPaging.PageIndex - 1) * currentPaging.PageSize).Take(currentPaging.PageSize).ToList();
            }
            else retVal = query.OrderBy(c => c.DisplayName).ToList();
            return retVal;
        }
    }
}
