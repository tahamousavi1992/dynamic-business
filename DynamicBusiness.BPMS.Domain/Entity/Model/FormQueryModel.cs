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

        public static IEnumerable<SqlParameter> GetSqlParameter(List<QueryModel> list)
        {
            return list?.Select(c => new SqlParameter("@" + c.Key, c.Value));
        }

        public static IEnumerable<SqlParameter> GetSqlParameter(List<QueryModel> list, string sqlQuery, Guid? threadId, Guid? processId)
        {

            List<SqlParameter> queryParams = list?.Select(c => new SqlParameter("@" + c.Key.TrimStart('@'), c.Value))?.ToList() ??
                new List<SqlParameter>();

            if (sqlQuery.Contains("@ThreadID"))
                queryParams.Add(new SqlParameter("@ThreadID", threadId));

            if (sqlQuery.Contains("@ProcessID"))
                queryParams.Add(new SqlParameter("@ProcessID", processId));

            queryParams = queryParams.Where(c => sqlQuery.Contains(c.ParameterName)).ToList();

            return queryParams;
        }

        public static List<string> ForbidenList
        {
            get { return new List<string>() { "skinsrc", "containersrc", "language", "controller", "action", "moduleid", "tabid", "amp;skinsrc", "controlid" }; }
        }
    }

}
