using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DynamicBusiness.BPMS.Domain
{
    public class QueryModel
    {
        public QueryModel() { }
        public QueryModel(string key, object value)
        {
            this.Key = key;
            this.Value = value;
        }
        public string Key { get; set; }
        public object Value { get; set; }


        public static IEnumerable<QueryModel> GetFormDataList(HttpRequest httpRequest)
        {
            if (httpRequest != null)
            {
                foreach (string key in httpRequest.Form.AllKeys.Where(c => !QueryModel.ForbidenList.Contains(c.ToLower())))
                    yield return new QueryModel() { Key = key.TrimStart('@'), Value = httpRequest.Form[key] };
                foreach (string key in httpRequest.Files.AllKeys.Where(c => !QueryModel.ForbidenList.Contains(c.ToLower())))
                    yield return new QueryModel() { Key = key.TrimStart('@'), Value = httpRequest.Files[key] };
            }
        }

        public static IEnumerable<SqlParameter> GetSqlParameter(List<QueryModel> list, string sqlQuery = null)
        {
            if (string.IsNullOrWhiteSpace(sqlQuery))
                return list?.Select(c => new SqlParameter("@" + c.Key.TrimStart('@'), c.Value));
            else
                return list?.Select(c => new SqlParameter("@" + c.Key.TrimStart('@'), c.Value))?.Where(c => sqlQuery.Contains(c.ParameterName)).ToList() ??
                       new List<SqlParameter>();
        }

        public static List<string> ForbidenList
        {
            get { return new List<string>() { "skinsrc", "containersrc", "language", "controller", "action", "moduleid", "tabid", "amp;skinsrc", "controlid" }; }
        }
    }

}
