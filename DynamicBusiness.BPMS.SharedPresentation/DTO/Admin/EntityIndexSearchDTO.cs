using DynamicBusiness.BPMS.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace DynamicBusiness.BPMS.SharedPresentation
{
    /// <summary>
    ///Controller -> BpmsEntityManager  Action -> Index
    /// </summary>
    [KnownType(typeof(EntityIndexSearchDTO))]
    [DataContract]
    public class EntityIndexSearchDTO : BaseSearchDTO<EntityDefDTO>
    {
        public EntityIndexSearchDTO() : base() { }
        //search region. 
        [DataMember]
        public string Name { get; set; }
    }
}