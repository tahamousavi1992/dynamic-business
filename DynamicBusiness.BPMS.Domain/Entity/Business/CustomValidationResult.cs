using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    public class CustomValidationResult
    {
        public CustomValidationResult()
        {
            this.ErrorList = new List<string>();
        }
        public bool IsValid { get { return this.ErrorList.Count == 0; } }
        public List<string> ErrorList { get; set; }
        public void AddError(string errorText)
        {
            this.ErrorList.Add(errorText);
        }
        public string GetErrors()
        {
            return string.Join("<br>", ErrorList);
        }
    }
}
