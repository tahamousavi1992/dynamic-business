using DynamicBusiness.BPMS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace DynamicBusiness.BPMS.SharedPresentation
{
    [DataContract]
    public class PostAddEditEntityDefDTO
    {
        [DataMember]
        public EntityDefDTO EntityDefDTO { get; set; }
        [DataMember]
        public List<EntityPropertyModel> listProperties { get; set; }
        [DataMember]
        public List<EntityRelationModel> listRelations { get; set; }
    }
}