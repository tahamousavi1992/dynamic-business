using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    [Serializable]
    [DataContract]
    public class DesignCodeModel
    {
        [DataMember]
        public string Code { get; set; }
        [DataMember]
        public string DesignCode { get; set; }
        [DataMember]
        public string Diagram { get; set; }
        /// <summary>
        ///after rendering all expression the assemblies are sum with each other and reserved into this property
        /// </summary>
        [DataMember]
        public string Assemblies { get; set; }
        [DataMember]
        public string ID { get; set; }
        [DataMember]
        public string TimeStamp { get; set; }//yyyyMMddHHmmssffff

        /// <summary>
        /// each item is an action or an expression code.(like call a method ,set variable,code expression)
        /// </summary>
        public List<object> CodeObjects
        {
            get
            {
                return DesignCodeUtility.GetListOfDesignCode(this.DesignCode);
            }
        }

    }
}
