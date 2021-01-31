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
    public class SubTypeMessageEventModel
    {
        public enum e_Type
        {
            [Description("Message")]
            Message = 1,
            [Description("Email")]
            Email = 2,
        }
        public enum e_KeyType
        {
            [Description("Variable")]
            Variable = 1,
            [Description("Static")]
            Static = 2,
        }
        public SubTypeMessageEventModel()
        {
            this.Type = (int)e_Type.Message;
        }
        [DataMember]
        public int? Type { get; set; }
        [DataMember]
        /// <summary>
        /// It is used for Matching two Message
        /// </summary> 
        public string Key { get; set; }
        [DataMember]
        /// <summary>
        /// It is used for Matching two Message
        /// </summary> 
        public int? KeyType { get; set; }
        [DataMember]
        /// <summary>
        /// e_Type.Message
        /// </summary>
        public List<SubTypeMessageParamEventModel> MessageParams { get; set; }
        [DataMember]
        /// <summary>
        /// e_Type.Email
        /// </summary>
        public SubTypeEmailEventModel Email { get; set; }
    }
}
