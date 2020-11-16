using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    [Serializable]
    public partial class sysBpmsDBConnection
    {
        public ResultOperation Update(Guid ID, string Name, string DataSource, string InitialCatalog, string UserID, string Password, bool IntegratedSecurity)
        {
            this.ID = ID;
            this.Name = Name;
            this.DataSource = DataSource;
            this.InitialCatalog = InitialCatalog;
            this.UserID = UserID;
            this.Password = Password;
            this.IntegratedSecurity = IntegratedSecurity;

            ResultOperation resultOperation = new ResultOperation(this);
            if (string.IsNullOrWhiteSpace(this.InitialCatalog))
                resultOperation.AddError(SharedLang.GetReuired(nameof(sysBpmsDBConnection.Name), nameof(sysBpmsDBConnection)));
            if (string.IsNullOrWhiteSpace(this.DataSource))
                resultOperation.AddError(SharedLang.GetReuired(nameof(sysBpmsDBConnection.DataSource), nameof(sysBpmsDBConnection)));

            return resultOperation;

        }

        public void Load(sysBpmsDBConnection DBConnection)
        {
            this.ID = DBConnection.ID;
            this.Name = DBConnection.Name;
            this.DataSource = DBConnection.DataSource;
            this.InitialCatalog = DBConnection.InitialCatalog;
            this.UserID = DBConnection.UserID;
            this.Password = DBConnection.Password;
            this.IntegratedSecurity = DBConnection.IntegratedSecurity;
        }

        public string GetConnection
        {
            get { return string.Format("Data Source={0};Initial Catalog={1};User ID={2};Password={3}", this.DataSource, this.InitialCatalog, this.UserID, this.Password); }
        }

    }
}
