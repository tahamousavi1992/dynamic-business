using DynamicBusiness.BPMS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public static class JsonExtentions
    {
        public static string AsJson(this List<ComboTreeModel> list)
        {
            return new JavaScriptSerializer().Serialize(list).Replace("\"subs\":[]", "").Replace(",}", "}");
        }
 
    }
}
