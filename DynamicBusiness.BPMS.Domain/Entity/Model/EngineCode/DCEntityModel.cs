using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DynamicBusiness.BPMS.Domain
{
    [DataContract]
    public class DCEntityModel : DCBaseModel
    {
        public DCEntityModel() { }
        public DCEntityModel(string id, string name, string shapeid, string parentShapeId, bool? isOutputYes,
            List<DCEntityParametersModel> rows, bool isFirst, int defaultMethodType)
            : base(id, name, e_ActionType.Entity, parentShapeId, shapeid, isOutputYes, isFirst, null)
        {
            this.Rows = rows;
            this.MethodType = (e_MethodType)defaultMethodType;
        }
        [DataMember]
        public Guid EntityDefID { get; set; }
        [DataMember]
        public List<DCEntityParametersModel> Rows { get; set; }

        [DataMember]
        public e_MethodType MethodType { get; set; }

        public override object FillData(XElement xElement)
        {
            base.FillData(xElement);
            this.EntityDefID = xElement.GetValue(nameof(DCEntityModel.EntityDefID)).ToGuidObj();
            this.MethodType = (e_MethodType)xElement.GetValue(nameof(DCEntityModel.MethodType)).ToIntObj();
            this.Rows = (from c in xElement.Element(nameof(DCEntityModel.Rows)).Elements(nameof(DCEntityParametersModel))
                         select new DCEntityParametersModel()
                         {
                             ParameterName = c.GetValue(nameof(DCEntityParametersModel.ParameterName)),
                             Value = c.GetValue(nameof(DCEntityParametersModel.Value)),
                             ValueType = (DCBaseModel.e_ValueType)c.GetValue(nameof(DCEntityParametersModel.ValueType)).ToIntObj(),
                         }).ToList();
            return this;
        }

        public override XElement ToXmlElement()
        {
            return new XElement(nameof(DCEntityModel),
                     base.ToXmlElementArray(),
                     new XElement(nameof(DCEntityModel.MethodType), (int)this.MethodType),
                     new XElement(nameof(DCEntityModel.EntityDefID), this.EntityDefID),
                     new XElement(nameof(DCEntityModel.Rows),
                     from c in this.Rows
                     select new XElement(nameof(DCEntityParametersModel),
                         new XElement(nameof(DCEntityParametersModel.ParameterName), c.ParameterName),
                         new XElement(nameof(DCEntityParametersModel.ValueType), (int)c.ValueType),
                         new XElement(nameof(DCEntityParametersModel.Value), c.Value)
                         ))
                     );
        }

        public override string GetRenderedCode(Guid? processId, Guid? applicationPageId, IUnitOfWork unitOfWork)
        {
            string code = string.Empty;
            sysBpmsEntityDef sysBpmsEntityDef = unitOfWork.Repository<IEntityDefRepository>().GetInfo(this.EntityDefID);
            switch (this.MethodType)
            {
                case e_MethodType.Create:
                    code += $@"VariableModel entityModel = new VariableModel(""{sysBpmsEntityDef.Name}"", new DataModel());
{
                        string.Join(Environment.NewLine, sysBpmsEntityDef.AllProperties.Select(c =>
"entityModel[" + c.Name + "]=" + this.GetParameterCode(this.Rows.FirstOrDefault(d => d.ParameterName == c.Name), c) + ";")) + Environment.NewLine}
this.EntityHelper.Save(entityModel);";
                    break;
                case e_MethodType.Update:
                    code += $@"VariableModel entityModel = new VariableModel(""{sysBpmsEntityDef.Name}"", new DataModel());
{
                        string.Join(Environment.NewLine, sysBpmsEntityDef.AllProperties.Select(c =>
"entityModel[" + c.Name + "]=" + this.GetParameterCode(this.Rows.FirstOrDefault(d => d.ParameterName == c.Name), c) + ";")) + Environment.NewLine}
this.EntityHelper.Save(entityModel);";
                    break;
                case e_MethodType.Delete:
                    code += $@"this.EntityHelper.DeleteById(""{sysBpmsEntityDef.Name}"",{this.GetParameterCode(this.Rows.FirstOrDefault(d => d.ParameterName == "ID"), sysBpmsEntityDef.AllProperties.FirstOrDefault(d => d.Name == "ID"))})";
                    break;
            }

            return code;
        }

        private string GetParameterCode(DCEntityParametersModel parameterModel, EntityPropertyModel entityProperty)
        {
            e_ConvertType e_Convert = e_ConvertType.String;
            string pn = parameterModel.ParameterName;
            switch (this.MethodType)
            {
                case e_MethodType.Create:
                    e_Convert = this.ConvertStrToType(entityProperty.DbType);
                    break;
                case e_MethodType.Update:
                    e_Convert = this.ConvertStrToType(entityProperty.DbType);
                    break;
                case e_MethodType.Delete:
                    e_Convert = this.ConvertStrToType(entityProperty.DbType);
                    break;
            }
            return parameterModel.GetRendered(e_Convert, entityProperty.Required);
        }

        private e_ConvertType ConvertStrToType(EntityPropertyModel.e_dbType convert)
        {
            switch (convert)
            {
                case EntityPropertyModel.e_dbType.boolean: return e_ConvertType.Boolean;
                case EntityPropertyModel.e_dbType.Integer: return e_ConvertType.Integer;
                case EntityPropertyModel.e_dbType.DateTime: return e_ConvertType.DateTime;
                case EntityPropertyModel.e_dbType.Uniqueidentifier: return e_ConvertType.Uniqueidentifier;
                case EntityPropertyModel.e_dbType.Decimal: return e_ConvertType.Decimal;
                case EntityPropertyModel.e_dbType.String: return e_ConvertType.String;
                case EntityPropertyModel.e_dbType.Long: return e_ConvertType.Long;
                default: return e_ConvertType.Nothing;
            }
        }

        public enum e_MethodType
        {
            [Description("Create")]
            Create = 1,
            [Description("Update")]
            Update = 2,
            [Description("Delete")]
            Delete = 3,
        }
    }

    [DataContract]
    public class DCEntityParametersModel
    {
        [DataMember]
        public string ParameterName { get; set; }
        [DataMember]
        public DCBaseModel.e_ValueType ValueType { get; set; }
        [DataMember]
        public string Value { get; set; }
        /// <summary>
        /// Do not save into xml , it is filled in controller method.
        /// </summary>
        [DataMember]
        public bool IsRequired { get; set; }

        public string GetRendered(DCBaseModel.e_ConvertType e_Convert, bool required)
        {
            return DCBaseModel.RenderValueType(null, null, null, this.Value, this.ValueType, e_Convert, !required);
        }
    }
}
