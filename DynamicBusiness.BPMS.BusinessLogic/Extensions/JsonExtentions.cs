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
        public static string AsJsonByNameID(this List<sysBpmsVariable> list)
        {
            return new JavaScriptSerializer().Serialize(list.Select(c => new { text = c.Name, value = c.ID }).ToList());
        }
        public static string AsJsonByName(this List<sysBpmsVariable> list)
        {
            return new JavaScriptSerializer().Serialize(list.Select(c => new { text = c.Name, value = c.Name }).ToList());
        }
        public static string AsJson(this List<ComboTreeModel> list)
        {
            return new JavaScriptSerializer().Serialize(list).Replace("\"subs\":[]", "").Replace(",}", "}");
        }
        public static string AsJson(this List<sysBpmsDynamicForm> list)
        {
            return new JavaScriptSerializer().Serialize(list.Select(c => new { text = c.Name, value = c.ID }).ToList());
        }
        public static string AsJson(this List<sysBpmsOperation> list)
        {
            return new JavaScriptSerializer().Serialize(list.Select(c => new { text = c.Name, value = c.ID }).ToList());
        }
  
        /// <summary>
        /// for comboTree
        /// </summary>
        public static string AsJson(this Dictionary<string, string> list)
        {
            return new JavaScriptSerializer().Serialize(list.Select(c => new ComboTreeModel()
            {
                id = c.Key,
                title = c.Value,
            })).Replace("\"subs\":[]", "");
        }

    }
}
