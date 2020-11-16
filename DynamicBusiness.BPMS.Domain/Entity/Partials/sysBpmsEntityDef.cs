using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    [Serializable]
    public partial class sysBpmsEntityDef
    {
        public ResultOperation Update(string displayName, string name, string tableName, string designXML, bool isActive, List<EntityPropertyModel> properties, List<EntityRelationModel> relations)
        {
            if (this.ID == Guid.Empty)
                this.TableName = tableName;
            this.DisplayName = displayName;
            this.Name = name;
            this.DesignXML = designXML;
            this.IsActive = isActive;
            this.Properties = properties;
            this.Relations = relations;

            ResultOperation resultOperation = new ResultOperation(this);
            if ((this.Properties == null || !this.Properties.Any()))
            {
                resultOperation.AddError(LangUtility.Get("AtLeastOnePropert.Text", nameof(sysBpmsEntityDef)));
            }
            if (this.Name.ToLower().StartsWith("sys") ||
                this.Name.ToLower().EndsWith("model") ||
                this.Name.ToLower().EndsWith("vm") ||
                this.Name.ToLower().EndsWith("html"))
            {
                resultOperation.AddError(LangUtility.Get("NameError.Text", nameof(sysBpmsEntityDef)));
            }
            foreach (var Item in this.Properties)
            {
                if (string.IsNullOrWhiteSpace(Item.Name))
                {
                    resultOperation.AddError(SharedLang.GetReuired("PropertyName.Text", nameof(sysBpmsEntityDef)));
                }
                if (Item.Name.ToLower() == "id")
                    resultOperation.AddError(LangUtility.Get("PropertyNameError.Text", nameof(sysBpmsEntityDef)));
                if (Item.Name.ToLower() == "variablename")
                    resultOperation.AddError(LangUtility.Get("PropertyNameError.Text", nameof(sysBpmsEntityDef)));
                if (Item.Name.ToLower() == this.Name)
                    resultOperation.AddError(LangUtility.Get("PropertyNameError.Text", nameof(sysBpmsEntityDef)));

                if (Item.IsActive && this.Properties.Any(c => c.ID != Item.ID && c.Name == Item.Name))
                    resultOperation.AddError(LangUtility.Get("SameProperty.Text", nameof(sysBpmsEntityDef)));
            }
            if (string.IsNullOrWhiteSpace(this.DisplayName))
            {
                resultOperation.AddError(SharedLang.GetReuired(nameof(sysBpmsEntityDef.DisplayName), nameof(sysBpmsEntityDef)));
            }
            if (string.IsNullOrWhiteSpace(this.TableName))
            {
                resultOperation.AddError(SharedLang.GetReuired(nameof(sysBpmsEntityDef.TableName), nameof(sysBpmsEntityDef)));
            }
            if (string.IsNullOrWhiteSpace(this.Name))
            {
                resultOperation.AddError(SharedLang.GetReuired(nameof(sysBpmsEntityDef.Name), nameof(sysBpmsEntityDef)));
            }

            if (this.Relations != null)
                foreach (var Item in this.Relations)
                {
                    if (Item.EntityDefID == Guid.Empty)
                    {
                        resultOperation.AddError(SharedLang.GetReuired(nameof(EntityRelationModel.EntityDefID), nameof(sysBpmsEntityDef)));
                    }
                    if (string.IsNullOrWhiteSpace(Item.EntityDefPropertyID))
                    {
                        resultOperation.AddError(SharedLang.GetReuired(nameof(EntityRelationModel.EntityDefPropertyID), nameof(sysBpmsEntityDef)));
                    }
                    if (string.IsNullOrWhiteSpace(Item.PropertyID))
                    {
                        resultOperation.AddError(SharedLang.GetReuired(nameof(EntityRelationModel.PropertyID), nameof(sysBpmsEntityDef)));
                    }
                }

            return resultOperation;
        }

        public void Load(sysBpmsEntityDef EntityDef)
        {
            this.ID = EntityDef.ID;
            this.Name = EntityDef.Name;
            this.DisplayName = EntityDef.DisplayName;
            this.TableName = EntityDef.TableName;
            this.DesignXML = EntityDef.DesignXML;
            this.IsActive = EntityDef.IsActive;
        }


        public string GetCode()
        {
            string code = string.Empty;
            //add constructor
            code += $@" public {this.Name}(){{}}";
            code += $@" public {this.Name}(DynamicBusiness.BPMS.Domain.IUnitOfWork _unitOfWork){{ this.Init(_unitOfWork); }} ";
            //add init method.
            code += $@" public {this.Name} Init(DynamicBusiness.BPMS.Domain.IUnitOfWork _unitOfWork){{ base.InitialProperties(_unitOfWork); return this; }} ";
            code += $@" public string VariableName {{get;set;}}= ""{this.Name}""; ";
            foreach (var item in this.AllProperties)
            {
                code += $@" public {item.CSharpTypeName} {item.Name} {{get;set;}} ";
            }
            //Add get by id method.
            code += $@" public {this.Name} GetById(Guid guid){{return (({this.Name})this.EntityHelper.GetById(""{this.Name}"", guid)).Init(base.UnitOfWork);}} ";
            //Add delete by id method.
            code += $@" public void DeleteById(){{this.EntityHelper.DeleteById(""{this.Name}"", this.ID);}} ";
            //Add save method.
            code += $@" public void Save(){{this.EntityHelper.Save<{this.Name}>(this);}} ";

            //Add implicit methods.
            code += $@" public static implicit operator {this.Name}(System.Data.DataTable dataTable){{return (DynamicBusiness.BPMS.Domain.VariableModel)(dataTable);}} ";
            code += $@" public static implicit operator {this.Name}(DynamicBusiness.BPMS.Domain.VariableModel variableModel){{return DynamicBusiness.BPMS.Domain.VariableModel.ConvertTo<{this.Name}>(variableModel);}} ";
            code += $@" public static implicit operator DynamicBusiness.BPMS.Domain.VariableModel({this.Name} entityData){{return DynamicBusiness.BPMS.Domain.VariableModel.ConvertFrom<{this.Name}>(entityData);}} ";

            string classCode = $@"
   public class {this.Name} :DynamicBusiness.BPMS.CodePanel.CodeBase
   {{
        public override object ExecuteCode(){{throw new NotImplementedException();}} 
       {code}
   }}";
            return classCode;
        }

        private List<EntityPropertyModel> properties { get; set; }
        public List<EntityPropertyModel> Properties
        {
            get
            {
                if (this.properties == null)
                    this.properties = this.DesignXML.ParseXML<EntityDesignXmlModel>().EntityPropertyModel ?? new List<EntityPropertyModel>();
                return properties;
            }
            set
            {
                properties = value;
            }
        }

        /// <summary>
        ///retrieve custom properties with systemic one like ID and ThreadID
        /// </summary>
        public List<EntityPropertyModel> AllProperties
        {
            get
            {

                return new List<EntityPropertyModel>() {
                    new EntityPropertyModel("1", "ID", true, EntityPropertyModel.e_dbType.Uniqueidentifier, null, null),
                    new EntityPropertyModel("2", "ThreadID", false, EntityPropertyModel.e_dbType.Uniqueidentifier, null, null)
                }
                .Union(Properties).ToList();
            }
        }

        private List<EntityRelationModel> relations { get; set; }
        public List<EntityRelationModel> Relations
        {
            get
            {
                if (this.relations == null)
                    this.relations = this.DesignXML.ParseXML<EntityDesignXmlModel>().EntityRelationModel ?? new List<EntityRelationModel>();
                return relations;
            }
            set
            {
                relations = value;
            }
        }

        public string FormattedTableName
        {
            get { return "Bpms_" + this.TableName; }
        }
    }
}
