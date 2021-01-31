using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    [DataContract]
    public class SubTypeMessageParamEventModel
    {
        public SubTypeMessageParamEventModel() { }
        public SubTypeMessageParamEventModel(string variable, string name, bool isRequired)
        {
            this.Name = name;
            this.Variable = variable;
            this.IsRequired = IsRequired;
        }
        [DataMember] 
        public string Variable { get; set; }
        [DataMember] 
        public string Name { get; set; } 
        /// <summary>
        /// This is not saved in xml model because it must get from MessageType Table.
        /// This is only for showing in message event's setting.This is filled by MessageType Table
        /// </summary>
        [System.Xml.Serialization.XmlIgnore] 
        public bool? IsRequired { get; set; }
    }
}
