using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using DynamicBusiness.BPMS.Domain;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class DocumentFolderRepository : IDocumentFolderRepository
    {
        private Db_BPMSEntities Context;
        public DocumentFolderRepository(Db_BPMSEntities context)
        {
            this.Context = context;
        }

        public void Add(sysBpmsDocumentFolder documentFolder)
        {
            documentFolder.ID = Guid.NewGuid();
            this.Context.sysBpmsDocumentFolders.Add(documentFolder);
        }

        public void Update(sysBpmsDocumentFolder documentFolder)
        {
            this.Context.Entry(documentFolder).State = EntityState.Modified;
        }

        public void Delete(Guid DocumentFolderId)
        {
            sysBpmsDocumentFolder documentFolder = this.Context.sysBpmsDocumentFolders.FirstOrDefault(d => d.ID == DocumentFolderId);
            if (documentFolder != null)
            {
                this.Context.sysBpmsDocumentFolders.Remove(documentFolder);
            }
        }

        public sysBpmsDocumentFolder GetInfo(Guid ID)
        {
            return (from P in this.Context.sysBpmsDocumentFolders
                    where P.ID == ID
                    select P).AsNoTracking().FirstOrDefault();
        }

        public List<sysBpmsDocumentFolder> GetList(Guid? ParentDocumentFolderID, string NameOf, string DisplayName, bool? isActive)
        {
            NameOf = DomainUtility.toString(NameOf);
            DisplayName = DomainUtility.toString(DisplayName);

            List<sysBpmsDocumentFolder> rettVal = this.Context.sysBpmsDocumentFolders.Where(d =>
            (NameOf == string.Empty || d.NameOf.Contains(NameOf)) &&
            (!isActive.HasValue || d.IsActive == isActive) &&
            (DisplayName == string.Empty || d.DisplayName.Contains(DisplayName)) &&
            (!ParentDocumentFolderID.HasValue || d.DocumentFolderID == ParentDocumentFolderID)).OrderBy(c => c.DisplayName).AsNoTracking().ToList();

            return rettVal;
        }
    }
}
