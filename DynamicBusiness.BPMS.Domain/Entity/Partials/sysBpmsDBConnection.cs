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
            this.Name = Name.ToStringObj();//not null in sql;
            this.DataSource = DataSource;
            this.InitialCatalog = InitialCatalog;
            this.UserID = UserID.ToStringObj();//not null in sql
            this.Password = Password.ToStringObj();//not null in sql
            this.IntegratedSecurity = IntegratedSecurity;

            ResultOperation resultOperation = new ResultOperation(this);
            if (string.IsNullOrWhiteSpace(this.InitialCatalog))
                resultOperation.AddError(SharedLang.GetReuired(nameof(sysBpmsDBConnection.InitialCatalog), nameof(sysBpmsDBConnection)));
            if (string.IsNullOrWhiteSpace(this.DataSource))
                resultOperation.AddError(SharedLang.GetReuired(nameof(sysBpmsDBConnection.DataSource), nameof(sysBpmsDBConnection)));

            return resultOperation;

        } 
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public string GetConnection
        {
            get { return string.Format("Data Source={0};Initial Catalog={1};User ID={2};Password={3}", this.DataSource, this.InitialCatalog, this.UserID, this.Password); }
        }

    }
}
