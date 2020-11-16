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
    [KnownType(typeof(ThreadIndexSearchDTO))]
    [DataContract]
    public class GetListProcessSearchDTO : BaseSearchDTO<KartableProcessDTO>
    {
        public GetListProcessSearchDTO() : base() { }
        //table region.
        [DataMember]
        public override List<KartableProcessDTO> GetList { get; set; }
    }
}