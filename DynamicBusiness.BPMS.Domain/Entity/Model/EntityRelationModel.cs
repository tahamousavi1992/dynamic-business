using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    [DataContract]
    public class EntityRelationModel
    {
        [DataMember]
        public string ID { get; set; }
         
        [DataMember]
        public Guid EntityDefID { get; set; }
         
        [DataMember]
        public string EntityDefPropertyID { get; set; }
         
        [DataMember]
        public string PropertyID { get; set; }
         
        [DataMember]
        public string Description { get; set; }
         
        [DataMember]
        public string ConstraintName { get; set; }

        /// <summary>
        /// It is used for add edit form entity.
        /// </summary>
        [DataMember]
        public List<EntityPropertyModel> GetEntityDefProperties { get; set; }

    }
}
