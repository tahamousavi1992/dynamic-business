using DynamicBusiness.BPMS.BusinessLogic;
using DynamicBusiness.BPMS.BusinessLogic.Migrations;
using DynamicBusiness.BPMS.Domain;
using System;
using System.Data.Entity.Migrations;
using System.Net.Http;
using System.Threading.Tasks;

namespace Bpms.BusinessLogic.Test
{
    public class BaseIntegratedTest : IDisposable
    {
        private readonly bool _create_db = false;

        public BaseIntegratedTest(bool create_db = true)
        {
            this._create_db = create_db;
            DomainUtility.IsTestEnvironment = true;
            if (this._create_db)
            {
                DomainUtility.SetConnectionString("Data Source=localhost\\MSSQLSERVER01;Initial Catalog=Test_BPMS;user ID=sa;Password=123;MultipleActiveResultSets=True;");
                new Db_BPMSEntities().Database.CreateIfNotExists();
            }
        }

        public void Dispose()
        {
            if (_create_db)
                new Db_BPMSEntities().Database.Delete();
        }

    }
}
