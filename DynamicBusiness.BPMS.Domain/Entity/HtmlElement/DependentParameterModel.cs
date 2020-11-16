using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    [DataContract]
    [KnownType(typeof(DependentParameterModel))]
    public class DependentParameterModel
    {
        public DependentParameterModel(string name, string controlID, bool isDependent)
        {
            this.Name = name;
            this.ControlID = controlID;
            this.IsDependent = isDependent;
        }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string ControlID { get; set; }
        [DataMember]
        public bool IsDependent { get; set; }
    }
}
