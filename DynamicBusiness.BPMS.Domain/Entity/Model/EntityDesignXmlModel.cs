using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    [DataContract]
    public class EntityDesignXmlModel
    {
        [DataMember]
        public List<EntityPropertyModel> EntityPropertyModel { get; set; }
        [DataMember]
        public List<EntityRelationModel> EntityRelationModel { get; set; }
    }
}
