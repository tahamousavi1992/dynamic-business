using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    [Serializable]
    public partial class sysBpmsVariable
    {
        public ResultOperation Update(Guid? processID, Guid? applicationPageID, string name, int varTypeLU, Guid? entityDefID, string fieldName, string query, int? filterTypeLU, string collection, Guid? dbConnectionID, string defaultValue, string whereClause, string orderByClause)
        {
            this.ProcessID = processID;
            this.ApplicationPageID = applicationPageID;
            this.Name = name;
            this.VarTypeLU = varTypeLU;
            this.EntityDefID = entityDefID;
            this.FieldName = fieldName;
            this.Query = query;
            if (RelationTypeLU == (int)sysBpmsVariable.e_RelationTypeLU.Entity)
                this.FilterTypeLU = applicationPageID.HasValue ? (int)sysBpmsVariable.e_FilterTypeLU.AllEntities : filterTypeLU;
            this.DBConnectionID = dbConnectionID;
            this.Collection = collection;
            this.DefaultValue = defaultValue;
            this.WhereClause = whereClause;
            this.OrderByClause = orderByClause;

            ResultOperation resultOperation = new ResultOperation(this);
            if (string.IsNullOrWhiteSpace(this.Name))
                resultOperation.AddError(SharedLang.GetReuired(nameof(sysBpmsVariable.Name), nameof(sysBpmsVariable)));
            switch ((sysBpmsVariable.e_RelationTypeLU)this.RelationTypeLU)
            {
                case sysBpmsVariable.e_RelationTypeLU.Entity:
                    if (this.VarTypeLU != (int)sysBpmsVariable.e_VarTypeLU.Object &&
                        this.VarTypeLU != (int)sysBpmsVariable.e_VarTypeLU.List &&
                        string.IsNullOrWhiteSpace(this.FieldName))
                    {
                        resultOperation.AddError(SharedLang.GetReuired(nameof(sysBpmsVariable.FieldName), nameof(sysBpmsVariable)));
                    }
                    break;
                case sysBpmsVariable.e_RelationTypeLU.SqlQuery:
                    if (string.IsNullOrWhiteSpace(this.Query))
                    {
                        resultOperation.AddError(SharedLang.GetReuired(nameof(sysBpmsVariable.Query), nameof(sysBpmsVariable)));
                    }
                    break;
                case sysBpmsVariable.e_RelationTypeLU.Local:
                    if (this.VarTypeLU == (int)sysBpmsVariable.e_VarTypeLU.List && !this.Items.Any())
                    {
                        resultOperation.AddError(LangUtility.Get("AtleastOneItem.Text", nameof(sysBpmsVariable)));
                    }
                    if (this.VarTypeLU == (int)sysBpmsVariable.e_VarTypeLU.List && this.Items.Any(c => string.IsNullOrWhiteSpace(c.Key) || string.IsNullOrWhiteSpace(c.Text)))
                    {
                        resultOperation.AddError(LangUtility.Get("AllItemError.Text", nameof(sysBpmsVariable)));
                    }
                    break;
            }
            return resultOperation;
        }

        public void Load(sysBpmsVariable Variable)
        {
            this.ID = Variable.ID;
            this.ProcessID = Variable.ProcessID;
            this.ApplicationPageID = Variable.ApplicationPageID;
            this.Name = Variable.Name;
            this.VarTypeLU = Variable.VarTypeLU;
            this.EntityDefID = Variable.EntityDefID;
            this.FieldName = Variable.FieldName;
            this.Query = Variable.Query;
            this.FilterTypeLU = Variable.FilterTypeLU;
            this.DBConnectionID = Variable.DBConnectionID;
            this.Collection = Variable.Collection;
            this.DefaultValue = Variable.DefaultValue;
            this.WhereClause = Variable.WhereClause;
            this.OrderByClause = Variable.OrderByClause;
        }

        public int RelationTypeLU
        {
            get
            {
                if (this.EntityDefID.HasValue)
                    return (int)e_RelationTypeLU.Entity;
                if (!string.IsNullOrWhiteSpace(this.Query))
                    return (int)e_RelationTypeLU.SqlQuery;
                return (int)e_RelationTypeLU.Local;
            }
        }

        public enum e_RelationTypeLU
        {
            [Description("Local")]
            Local = 1,
            [Description("Entity")]
            Entity = 2,
            [Description("Query")]
            SqlQuery = 3,
        }

        public enum e_FilterTypeLU
        {
            [Description("Filter by thread")]
            Filtered = 1,
            [Description("All")]
            AllEntities = 2,
        }

        public enum e_VarTypeLU
        {
            [Description("Text")]
            String = 1,
            [Description("Number")]
            Integer = 2,
            [Description("Decimal")]
            Decimal = 3,
            [Description("List")]
            List = 4,
            [Description("Entity")]
            Object = 5,
            [Description("Date Time")]
            DateTime = 6,
            [Description("Boolean")]
            Boolean = 7,
            [Description("Uniqueidentifier")]
            Uniqueidentifier = 8,
        }

        public IEnumerable<VariableItemModel> Items
        {
            get
            {
                return this.Collection.ParseXML<List<VariableItemModel>>();
            }
        }

        public bool IsBindToOneData
        {
            get
            {
                switch ((sysBpmsVariable.e_VarTypeLU)this.VarTypeLU)
                {
                    case sysBpmsVariable.e_VarTypeLU.Decimal:
                    case sysBpmsVariable.e_VarTypeLU.Integer:
                    case sysBpmsVariable.e_VarTypeLU.String:
                    case sysBpmsVariable.e_VarTypeLU.DateTime:
                    case sysBpmsVariable.e_VarTypeLU.Boolean:
                    case sysBpmsVariable.e_VarTypeLU.Uniqueidentifier:
                        return true;
                    default: return false;
                }
            }
        }
    }
}
