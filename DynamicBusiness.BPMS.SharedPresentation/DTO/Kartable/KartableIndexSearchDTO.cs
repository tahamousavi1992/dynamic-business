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
    ///Controller -> KartableHome  Action -> Index
    /// </summary>
    [DataContract]
    [KnownType(typeof(KartableIndexSearchDTO))]
    public class KartableIndexSearchDTO : BaseSearchDTO<KartableDTO>
    {
        public KartableIndexSearchDTO() : base() { }
    }
}