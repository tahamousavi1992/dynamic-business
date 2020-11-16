using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    [DataContract]
    public class EventDataAttributesModel
    {
        [DataMember]
        public string AttrName { get; set; }
        [DataMember]
        public string FunctionName { get; set; }
    }
}
