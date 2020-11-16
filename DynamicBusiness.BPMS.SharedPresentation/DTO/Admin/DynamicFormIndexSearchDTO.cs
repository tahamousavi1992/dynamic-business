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
    ///Controller -> DynamicForm  Action -> Index
    /// </summary>
    [KnownType(typeof(DynamicFormIndexSearchDTO))]
    [DataContract]
    public class DynamicFormIndexSearchDTO : BaseSearchDTO<DynamicFormDTO>
    {
        public DynamicFormIndexSearchDTO() : base() { }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public Guid? ProcessId { get; set; }
    }
}