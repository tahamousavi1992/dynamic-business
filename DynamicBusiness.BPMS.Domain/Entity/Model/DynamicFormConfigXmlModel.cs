using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    public class DynamicFormConfigXmlModel
    {
        public string OnLoadFunctionBody { get; set; }
        public string StyleSheetCode { get; set; }
        public bool IsEncrypted { get; set; }
    }
}
