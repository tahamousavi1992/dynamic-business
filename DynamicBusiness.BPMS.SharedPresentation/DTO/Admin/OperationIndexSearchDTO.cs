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
    ///Controller -> Operation  Action -> Index
    /// </summary>
    [KnownType(typeof(OperationIndexSearchDTO))]
    [DataContract]
    public class OperationIndexSearchDTO : BaseSearchDTO<OperationDTO>
    {
        public OperationIndexSearchDTO() : base() { }
        [DataMember] 
        public string Name { get; set; }
    }
}