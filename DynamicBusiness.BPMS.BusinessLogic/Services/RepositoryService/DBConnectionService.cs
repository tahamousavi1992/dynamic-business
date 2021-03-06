﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicBusiness.BPMS.Domain;
namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class DBConnectionService : ServiceBase
    {
        public DBConnectionService(IUnitOfWork unitOfWork = null) : base(unitOfWork) { }

        public ResultOperation Add(sysBpmsDBConnection dBConnection)
        {
            ResultOperation resultOperation = new ResultOperation();
            if (this.GetList(dBConnection.Name).Any(c => c.ID != dBConnection.ID))
                resultOperation.AddError(LangUtility.Get("RepetitiveNameError.Text", nameof(sysBpmsDBConnection)));
            if (resultOperation.IsSuccess)
            {
                this.UnitOfWork.Repository<IDBConnectionRepository>().Add(dBConnection);
                this.UnitOfWork.Save();
            }
            return resultOperation;
        }

        public ResultOperation Update(sysBpmsDBConnection dBConnection)
        {
            ResultOperation resultOperation = new ResultOperation();
            if (this.GetList(dBConnection.Name).Any(c => c.ID != dBConnection.ID))
                resultOperation.AddError(LangUtility.Get("RepetitiveNameError.Text", nameof(sysBpmsDBConnection)));
            if (resultOperation.IsSuccess)
            {
                this.UnitOfWork.Repository<IDBConnectionRepository>().Update(dBConnection);
                this.UnitOfWork.Save();
            }
            return resultOperation;
        }

        public ResultOperation Delete(Guid DBConnectionId)
        {
            ResultOperation resultOperation = new ResultOperation();
            if (this.UnitOfWork.Repository<IVariableRepository>().GetList(DBConnectionId).Any())
            {
                resultOperation.AddError(LangUtility.Get("ConnectionUsed.Text", nameof(sysBpmsDBConnection)));
            }
            if (resultOperation.IsSuccess)
            {
                this.UnitOfWork.Repository<IDBConnectionRepository>().Delete(DBConnectionId);
                this.UnitOfWork.Save();
            }
            return resultOperation;
        }

        public sysBpmsDBConnection GetInfo(Guid ID)
        {
            return this.UnitOfWork.Repository<IDBConnectionRepository>().GetInfo(ID);
        }

        public List<sysBpmsDBConnection> GetList(string Name)
        {
            return this.UnitOfWork.Repository<IDBConnectionRepository>().GetList(Name);
        }

        public List<sysBpmsDBConnection> GetList(string Name, string DataSource, string InitialCatalog, PagingProperties currentPaging)
        {
            return this.UnitOfWork.Repository<IDBConnectionRepository>().GetList(Name, DataSource, InitialCatalog, currentPaging);
        }

        /// <summary>
        /// Creates a connection string by SqlConnectionStringBuilder and returns it.
        /// </summary>
        /// <returns></returns>
        public string GetSqlConnectionString(sysBpmsDBConnection DBConnection)
        {

            SqlConnectionStringBuilder connectionString = new SqlConnectionStringBuilder()
            {
                DataSource = DBConnection.DataSource,
                InitialCatalog = DBConnection.InitialCatalog,
                MultipleActiveResultSets = true,
            };

            // Check current Windows account credentials are used for authentication
            if (DBConnection.IntegratedSecurity)
                connectionString.IntegratedSecurity = true;
            else
            {
                connectionString.Password = DBConnection.Password;
                connectionString.UserID = DBConnection.UserID;
            }

            return connectionString.ConnectionString;
        }

        /// <summary>
        /// Tests connection string by opening it
        /// </summary>
        /// <returns></returns>
        public bool TestConnection(sysBpmsDBConnection DBConnection)
        {
            try
            {
                string connectionString = GetSqlConnectionString(DBConnection);
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open(); // throws if invalid
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}
