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
            [Description("مقدار ثابت")]
            Static = 1,//write email address.
            [Description("متغیر")]
            Variable = 2,//use variable.
            [Description("کاربر متناظر")]
            Systemic = 3,//use current user or current department
        }

        public enum e_ToSystemicType
        {
            [Description("کاربر فعلی")]
            CurrentUser = 1,
            [Description("کاربر درخواست کننده")]
            CurrentThreadUser = 2,
        }

        public enum e_FromType
        {
            CurrentUser = 1,
            CurrentThreadUser = 2,
        }
    }
}
