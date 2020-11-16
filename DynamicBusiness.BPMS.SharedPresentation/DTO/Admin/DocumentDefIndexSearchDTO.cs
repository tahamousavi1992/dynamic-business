using DynamicBusiness.BPMS.Domain;
using DynamicBusiness.BPMS.SharedPresentation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace DynamicBusiness.BPMS.SharedPresentation
{
    /// <summary>
    ///Controller -> DbManager  Action -> Index
    /// </summary>
    [KnownType(typeof(DocumentDefIndexSearchDTO))]
    [DataContract]
    public class DocumentDefIndexSearchDTO : BaseSearchDTO<DocumentDefDTO>
    {
        public DocumentDefIndexSearchDTO() : base() { }
        [DataMember] 
        public Guid? DocumentFolderID { get; set; }
    }
}