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
    [Serializable]
    [DataContract]
    public class SubTypeEmailEventModel
    {
        [DataMember]
        public string ID { get; set; }
        [DataMember] 
        public string Subject { get; set; }
        [DataMember] 
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }
        [DataMember] 
        public string From { get; set; }
        [DataMember] 
        public int ToType { get; set; }
        [DataMember] 
        public string To { get; set; }

        public enum e_ToType
        {
            [Description("Constant")]
            Static = 1,//write email address.
            [Description("Variable")]
            Variable = 2,//use variable.
            [Description("Corresponding user")]
            Systemic = 3,//use current user or current department
        }

        public enum e_ToSystemicType
        {
            [Description("Current User")]
            CurrentUser = 1,
            [Description("Requester User")]
            CurrentThreadUser = 2,
        }

        public enum e_FromType
        {
            CurrentUser = 1,
            CurrentThreadUser = 2,
        }
    }
}
