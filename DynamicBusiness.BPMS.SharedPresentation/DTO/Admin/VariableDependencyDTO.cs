using DynamicBusiness.BPMS.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.SharedPresentation
{
    [DataContract]
    public class VariableDependencyDTO
    {
        public VariableDependencyDTO() { }
        public VariableDependencyDTO(sysBpmsVariableDependency variableDependency)
        {
            this.ID = variableDependency.ID;
            this.DependentVariableID = variableDependency.DependentVariableID;
            this.DependentPropertyName = variableDependency.DependentPropertyName;
            this.ToVariableID = variableDependency.ToVariableID;
            this.ToPropertyName = variableDependency.ToPropertyName;
            this.Description = variableDependency.Description;
        }

        [DataMember]
        public Guid ID { get; set; }
         
        [DataMember]
        public Guid DependentVariableID { get; set; }
         
        [DataMember]
        public string DependentPropertyName { get; set; }
         
        [DataMember]
        public Guid? ToVariableID { get; set; }

        [DataMember]
        public List<EntityPropertyModel> GetToVariableProperties { get; set; }
         
        [DataMember]
        public string ToPropertyName { get; set; }
         
        [DataMember]
        public string Description { get; set; }
    }
}
