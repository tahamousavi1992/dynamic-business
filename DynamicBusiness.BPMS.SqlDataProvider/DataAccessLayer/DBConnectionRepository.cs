using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicBusiness.BPMS.Domain;
using System.Data.Entity;
namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class DBConnectionRepository : IDBConnectionRepository
    {
        private Db_BPMSEntities Context;
        public DBConnectionRepository(Db_BPMSEntities context)
        {
            this.Context = context;
        }

        public void Add(sysBpmsDBConnection DBConnection)
        {
            DBConnection.ID = Guid.NewGuid();
            this.Context.sysBpmsDBConnections.Add(DBConnection);
        }

        public void Update(sysBpmsDBConnection DBConnection)
        {
            sysBpmsDBConnection retVal = (from p in this.Context.sysBpmsDBConnections
                                      where p.ID == DBConnection.ID
                                   select p).FirstOrDefault();
            retVal.Load(DBConnection);
        }

        public void Delete(Guid DBConnectionId)
        {
            sysBpmsDBConnection DBConnection = this.Context.sysBpmsDBConnections.FirstOrDefault(d => d.ID == DBConnectionId);
            if (DBConnection != null)
            {
                this.Context.sysBpmsDBConnections.Remove(DBConnection);
            }
        }

        public sysBpmsDBConnection GetInfo(Guid ID)
        {
            return this.Context.sysBpmsDBConnections.AsNoTracking().FirstOrDefault(d => d.ID == ID); ;
        }

        public List<sysBpmsDBConnection> GetList(string Name)
        {
            Name = DomainUtility.toString(Name);
            return this.Context.sysBpmsDBConnections.Where(d =>
            (Name == string.Empty || d.Name == Name)).AsNoTracking().ToList();
        }
        public List<sysBpmsDBConnection> GetList(string Name, string DataSource, string InitialCatalog, PagingProperties currentPaging)
        {
            List<sysBpmsDBConnection> list = new List<sysBpmsDBConnection>();

            Name = Name.ToStringObj();
            DataSource = DataSource.ToStringObj();
            InitialCatalog = InitialCatalog.ToStringObj();

            var query = (from P in this.Context.sysBpmsDBConnections
                         where
                         (Name == string.Empty || P.Name.Contains(Name)) &&
                         (DataSource == string.Empty || P.DataSource.Contains(DataSource)) &&
                         (InitialCatalog == string.Empty || P.InitialCatalog.Contains(InitialCatalog))
                         select P).AsNoTracking();


            if (currentPaging != null)
            {
                currentPaging.RowsCount = query.Count();
                if (Math.Ceiling(Convert.ToDecimal(currentPaging.RowsCount) / Convert.ToDecimal(currentPaging.PageSize)) < currentPaging.PageIndex)
                    currentPaging.PageIndex = 1;

                list = query.OrderByDescending(p => p.ID).Skip((currentPaging.PageIndex - 1) * currentPaging.PageSize).Take(currentPaging.PageSize).ToList();
            }
            else list = query.ToList();

            return list;
        }
    }
}
