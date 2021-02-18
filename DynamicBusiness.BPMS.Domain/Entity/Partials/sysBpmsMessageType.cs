using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    [Serializable]
    public partial class sysBpmsMessageType
    {
        public ResultOperation Update(string name, bool isActive, List<MessageTypeParamsModel> paramsXML)
        {
            paramsXML = paramsXML?.Where(c => !string.IsNullOrWhiteSpace(c.Name)).ToList();
            ResultOperation resultOperation = new ResultOperation();
            this.Name = name;
            this.IsActive = isActive;
            this.ParamsXML = paramsXML.BuildXml() ?? "";
            if (paramsXML?.GroupBy(c => c.Name).Any(c => c.Count() > 1) ?? false)
                resultOperation.AddError(LangUtility.Get("SameName.Text", nameof(sysBpmsMessageType)));
            return resultOperation;
        }
 
        [NotMapped]
        public List<MessageTypeParamsModel> ParamsXmlModel
        {
            get => this.ParamsXML.ParseXML<List<MessageTypeParamsModel>>() ?? new List<MessageTypeParamsModel>();

        }
    }
}
