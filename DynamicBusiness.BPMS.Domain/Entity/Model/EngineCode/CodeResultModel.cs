using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    [DataContract]
    public class CodeResultModel
    {
        public CodeResultModel() { }
        public CodeResultModel(bool result,
             RedirectUrlModel redirectUrlModel, CodeBaseSharedModel codeBaseShared)
        {
            this.Result = result;
            this.RedirectUrlModel = redirectUrlModel;
            this.CodeBaseShared = codeBaseShared;
        }
        [DataMember]
        public bool Result { get; set; }
        [DataMember]
        public RedirectUrlModel RedirectUrlModel { get; set; }
        [DataMember]
        public CodeBaseSharedModel CodeBaseShared { get; set; }
    }
}
