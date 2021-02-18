using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using DynamicBusiness.BPMS.Domain;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class DocumentRepository : IDocumentRepository
    {
        private Db_BPMSEntities Context;
        public DocumentRepository(Db_BPMSEntities context)
        {
            this.Context = context;
        }

        public void Add(sysBpmsDocument document)
        {
            this.Context.sysBpmsDocuments.Add(document);
        }

        public void Update(sysBpmsDocument document)
        {
            this.Context.Entry(document).State = EntityState.Modified;
        }

        public void Delete(Guid Guid)
        {
            sysBpmsDocument document = this.Context.sysBpmsDocuments.FirstOrDefault(d => d.GUID == Guid);
            if (document != null)
            {
                this.Context.sysBpmsDocuments.Remove(document);
            }
        }

        public sysBpmsDocument GetInfo(Guid Guid)
        {
            return (from P in this.Context.sysBpmsDocuments
                    where P.GUID == Guid
                    select P).AsNoTracking().FirstOrDefault();
        }

        public List<sysBpmsDocument> GetList(Guid? DocumentDefID, Guid? EntityDefID, Guid? EntityID, string CaptionOf, bool? IsDeleted, Guid? DocumentFolderID, Guid? threadId)
        {
            CaptionOf = DomainUtility.toString(CaptionOf);

            List<sysBpmsDocument> rettVal = this.Context.sysBpmsDocuments.Where(d =>
            (CaptionOf == string.Empty || d.CaptionOf.Contains(CaptionOf)) &&
            (!IsDeleted.HasValue || d.IsDeleted == IsDeleted) &&
            (!EntityDefID.HasValue || d.EntityDefID == EntityDefID) &&
            (!EntityID.HasValue || d.EntityID == EntityID) &&
            (!DocumentDefID.HasValue || d.DocumentDefID == DocumentDefID) &&
            (!threadId.HasValue || d.ThreadID == threadId) &&
            (!DocumentFolderID.HasValue || d.DocumentDef.DocumentFolderID == DocumentFolderID)).AsNoTracking().ToList();

            return rettVal;
        }
    }
}
