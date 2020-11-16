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
    ///Controller -> APIAccessIndex  Action -> Index
    /// </summary>
    [KnownType(typeof(APIAccessIndexSearchDTO))]
    [DataContract]
    public class APIAccessIndexSearchDTO : BaseSearchDTO<APIAccessDTO>
    {
        public APIAccessIndexSearchDTO() : base() { }
        [DataMember]
        public string Name { get; set; }
    }
}