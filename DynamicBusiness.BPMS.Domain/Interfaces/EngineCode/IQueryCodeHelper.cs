using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    public interface IQueryCodeHelper
    {
        DataTable Get(string sqlQuery, params QueryModel[] _params);

        int Execute(string sqlQuery, params QueryModel[] _params);

        T ExecuteScalar<T>(string sqlQuery, params QueryModel[] _params);
    }
}
