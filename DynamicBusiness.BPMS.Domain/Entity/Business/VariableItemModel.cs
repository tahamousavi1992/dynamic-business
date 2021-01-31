using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace DynamicBusiness.BPMS.Domain
{
    [DataContract]
    public class VariableItemModel
    { 
        [DataMember]
        public string Key { get; set; }
        [DataMember]
        public string Text { get; set; }

        public VariableItemModel()
        {
            this.Key = string.Empty;
            this.Text = string.Empty;
        }
    }
}
