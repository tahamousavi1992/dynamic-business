using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    public class TableSchemaModel
    {
        public TableSchemaModel(DataRow row)
        {
            this.TABLE_NAME = row["TABLE_NAME"].ToString();
            this.ORDINAL_POSITION = DomainUtility.toInt(row["ORDINAL_POSITION"]);
            this.COLUMN_NAME = row["COLUMN_NAME"].ToString();
            this.IS_NULLABLE = row["IS_NULLABLE"].ToString() == "YES";
            this.DATA_TYPE = row["DATA_TYPE"].ToString() == "int" ? EntityPropertyModel.e_dbType.Integer :
                row["DATA_TYPE"].ToString() == "decimal" ? EntityPropertyModel.e_dbType.Decimal :
                row["DATA_TYPE"].ToString() == "bigint" ? EntityPropertyModel.e_dbType.Long :
                row["DATA_TYPE"].ToString() == "nvarchar" ? EntityPropertyModel.e_dbType.String : EntityPropertyModel.e_dbType.String;
            this.CHARACTER_MAXIMUM_LENGTH = row["CHARACTER_MAXIMUM_LENGTH"].ToString();
        }
        public string TABLE_NAME { get; set; }
        public int ORDINAL_POSITION { get; set; }
        public string COLUMN_NAME { get; set; }
        public bool IS_NULLABLE { get; set; }
        public EntityPropertyModel.e_dbType DATA_TYPE { get; set; }
        public string CHARACTER_MAXIMUM_LENGTH { get; set; }
    }
}
