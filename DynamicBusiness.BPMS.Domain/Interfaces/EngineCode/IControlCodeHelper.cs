using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    public interface IControlCodeHelper
    {
        void SetValue(string controlId, object value);
        void BindDataSource(string controlId, object value);
        object GetValue(string controlId);
        void SetVisibility(string controlId, bool visibility);
    }
}
