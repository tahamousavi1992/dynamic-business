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
    [KnownType(typeof(ProcessIndexSearchDTO))]
    [DataContract]
    public class ProcessIndexSearchDTO : BaseSearchDTO<ProcessDTO>
    {
        public ProcessIndexSearchDTO() : base() { }
        public Guid? SelectedID { get; set; }
    }
}