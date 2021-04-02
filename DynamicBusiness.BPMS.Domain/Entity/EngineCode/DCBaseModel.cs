using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace DynamicBusiness.BPMS.Domain
{
    [Serializable]
    [DataContract]
    public class DCBaseModel
    {
        public DCBaseModel() { }
        public DCBaseModel(string id, string name, e_ActionType actionType, string parentShapeID, string shapeID, bool? isOutputYes, bool isFirst, string funcName)
        {
            this.ID = id;
            this.Name = string.IsNullOrWhiteSpace(name) ? "Action" : name;
            this.FuncName = string.IsNullOrWhiteSpace(funcName) ? DesignCodeUtility.GetFunctionName(shapeID) : funcName;
            this.ActionType = actionType;
            this.ParentShapeID = parentShapeID;
            this.ShapeID = shapeID;
            this.IsOutputYes = isOutputYes;
            this.IsFirst = isFirst;
        }
        [DataMember]
        public string ID { get; set; }
        [DataMember]
        [Required]
        public string Name { get; set; }
        [DataMember]
        [Required]
        public string FuncName { get; set; }
        [DataMember]
        public e_ActionType ActionType { get; set; }
        [DataMember]
        public string ParentShapeID { get; set; }
        [DataMember]
        public string ShapeID { get; set; }
        [DataMember]
        public bool IsFirst { get; set; }
        /// <summary>
        /// does it comes from a yes output connection from a condition shape.
        /// </summary>
        [DataMember]
        public bool? IsOutputYes { get; set; }
        [DataMember]
        public string ActionTypeName { get { return this.ActionType.GetDescription(); } set { } }
        public enum e_ActionType
        {
            [Description("SetVariable")]
            [XmlEnum("1")] SetVariable = 1,
            [Description("Condition")]
            [XmlEnum("2")] Condition = 2,
            [Description("CallMethod")]
            [XmlEnum("3")] CallMethod = 3,
            [Description("Expression")]
            [XmlEnum("4")] Expression = 4,
            [Description("SetControl")]
            [XmlEnum("5")] SetControl = 5,
            [Description("WebService")]
            [XmlEnum("7")] WebService = 7,
            [Description("Entity")]
            [XmlEnum("8")] Entity = 8,
            [Description("SqlFunction")]
            [XmlEnum("9")] SqlFunction = 9,
            [Description("Email")]
            [XmlEnum("10")] Email = 10,
        }
        public virtual object FillData(XElement xElement)
        {
            this.ID = xElement.GetValue(nameof(DCBaseModel.ID));
            this.ShapeID = xElement.GetValue(nameof(DCBaseModel.ShapeID));
            this.ParentShapeID = xElement.GetValue(nameof(DCBaseModel.ParentShapeID));
            this.Name = xElement.GetValue(nameof(DCBaseModel.Name));
            this.FuncName = xElement.GetValue(nameof(DCBaseModel.FuncName));
            this.ActionType = (DCBaseModel.e_ActionType)xElement.GetValue(nameof(DCBaseModel.ActionType)).ToIntObj();
            this.IsOutputYes = xElement.GetValue(nameof(DCBaseModel.IsOutputYes)).ToLower() == "true";
            this.IsFirst = xElement.GetValue(nameof(DCBaseModel.IsFirst)).ToLower() == "true";
            return this;
        }
        public virtual XElement ToXmlElement()
        {
            return null;
        }
        public XElement[] ToXmlElementArray()
        {
            return new XElement[]{ new XElement(nameof(DCBaseModel.ID), this.ID),
                   new XElement(nameof(DCBaseModel.ActionType), (int)this.ActionType),
                   new XElement(nameof(DCBaseModel.Name), this.Name),
                   new XElement(nameof(DCBaseModel.FuncName), this.FuncName),
                   new XElement(nameof(DCBaseModel.ParentShapeID), this.ParentShapeID),
                   new XElement(nameof(DCBaseModel.ShapeID), this.ShapeID),
                   new XElement(nameof(DCBaseModel.IsOutputYes), this.IsOutputYes),
                   new XElement(nameof(DCBaseModel.IsFirst), this.IsFirst)};
        }

        public virtual bool Execute(ICodeBase codeBase)
        {
            return true;
        }

        /// <summary>
        /// this method would return variable convert type like string,int,boo,... according to variable name
        /// </summary>
        public static e_ConvertType GetVariableConvertType(string variableName, Guid? processId, Guid? applicationPageId, IUnitOfWork unitOfWork)
        {
            e_ConvertType e_Convert = e_ConvertType.Integer;
            sysBpmsVariable sysBpmsVariable = unitOfWork.Repository<IVariableRepository>().GetInfo(processId, applicationPageId, variableName.Split('.')[0], null); ;
            if (sysBpmsVariable != null)
            {
                switch ((sysBpmsVariable.e_VarTypeLU)sysBpmsVariable.VarTypeLU)
                {
                    case sysBpmsVariable.e_VarTypeLU.String:
                        e_Convert = e_ConvertType.String;
                        break;
                    case sysBpmsVariable.e_VarTypeLU.Uniqueidentifier:
                        e_Convert = e_ConvertType.Uniqueidentifier;
                        break;
                    case sysBpmsVariable.e_VarTypeLU.Integer:
                        e_Convert = e_ConvertType.Integer;
                        break;
                    case sysBpmsVariable.e_VarTypeLU.Decimal:
                        e_Convert = e_ConvertType.Decimal;
                        break;
                    case sysBpmsVariable.e_VarTypeLU.List:

                        break;
                    case sysBpmsVariable.e_VarTypeLU.Object:
                        switch ((sysBpmsVariable.e_RelationTypeLU)sysBpmsVariable.RelationTypeLU)
                        {
                            case sysBpmsVariable.e_RelationTypeLU.Entity:
                                sysBpmsEntityDef sysEntity = unitOfWork.Repository<IEntityDefRepository>().GetInfo(sysBpmsVariable.EntityDefID.Value);
                                if (sysEntity != null && variableName.Split('.').Count() == 2)
                                {
                                    EntityPropertyModel propertyModel = sysEntity.AllProperties.FirstOrDefault(c => c.Name == variableName.Split('.')[1]);
                                    if (propertyModel != null)
                                    {
                                        switch (propertyModel.DbType)
                                        {
                                            case EntityPropertyModel.e_dbType.String: e_Convert = e_ConvertType.String; break;
                                            case EntityPropertyModel.e_dbType.Integer: e_Convert = e_ConvertType.Integer; break;
                                            case EntityPropertyModel.e_dbType.Decimal: e_Convert = e_ConvertType.Decimal; break;
                                            case EntityPropertyModel.e_dbType.Long: e_Convert = e_ConvertType.Long; break;
                                            case EntityPropertyModel.e_dbType.DateTime: e_Convert = e_ConvertType.DateTime; break;
                                            case EntityPropertyModel.e_dbType.Uniqueidentifier: e_Convert = e_ConvertType.Uniqueidentifier; break;
                                            case EntityPropertyModel.e_dbType.boolean: e_Convert = e_ConvertType.Boolean; break;
                                        }
                                    }
                                }
                                break;
                        }
                        break;
                    case sysBpmsVariable.e_VarTypeLU.DateTime:
                        e_Convert = e_ConvertType.DateTime;
                        break;
                    case sysBpmsVariable.e_VarTypeLU.Boolean:
                        e_Convert = e_ConvertType.Boolean;
                        break;
                }
            }
            return e_Convert;
        }

        public static object GetConverted(object value, e_ConvertType e_Convert, bool allowNull = true)
        {
            switch (e_Convert)
            {
                case e_ConvertType.String:
                    return (value.ToStringObj().ToLower().Trim() == "null" || value.ToStringObj().ToLower().Trim() == "\"null\"") ? null : (!allowNull ? value.ToStringObj() : value.ToStringObjNull());
                case e_ConvertType.Integer:
                    return !allowNull ? value.ToIntObj() : value.ToIntObjNull();
                case e_ConvertType.Decimal:
                    return !allowNull ? value.ToDecimalObj() : value.ToDecimalObjNull();

                case e_ConvertType.DateTime:
                    return !allowNull ? value.ToDateTimeObj() : value.ToDateTimeObjNull();

                case e_ConvertType.Boolean:
                    return !allowNull ? value.ToBoolObj() : value.ToBoolObjNull();

                case e_ConvertType.Long:
                    return !allowNull ? value.ToLongObj() : value.ToLongObjNull();

                case e_ConvertType.Uniqueidentifier:
                    return !allowNull ? value.ToGuidObj() : value.ToGuidObjNull();
            }
            return value;
        }

        public static object GetValue(ICodeBase codeBase, string value, e_ValueType valueType,
           DCBaseModel.e_ConvertType e_Convert, bool allowNull = true)
        {
            switch (valueType)
            {
                case e_ValueType.Static:
                    return DCBaseModel.GetConverted(value, e_Convert, allowNull);
                case e_ValueType.Control:
                    return DCBaseModel.GetConverted(codeBase.ControlHelper.GetValue(value), e_Convert, allowNull);
                case e_ValueType.Parameter:
                    return DCBaseModel.GetConverted(codeBase.UrlHelper.GetParameter(value), e_Convert, allowNull);
                case e_ValueType.Variable:
                    return DCBaseModel.GetConverted(codeBase.VariableHelper.GetValue(value), e_Convert, allowNull);
                case e_ValueType.SysParameter:
                    switch (value)
                    {
                        case "GetCurrentUserID":
                            return DCBaseModel.GetConverted(codeBase.GetCurrentUserID, e_Convert, allowNull);
                        case "GetCurrentUserName":
                            return DCBaseModel.GetConverted(codeBase.GetCurrentUserName, e_Convert, allowNull);
                        case "GetThreadID":
                            return DCBaseModel.GetConverted(codeBase.GetThreadID, e_Convert, allowNull);
                        case "GetThreadUserID":
                            return DCBaseModel.GetConverted(codeBase.GetThreadUserID, e_Convert, allowNull);
                    }
                    break;

            }
            return null;
        }


        public enum e_ConvertType
        {
            Nothing = 0,
            String = 1,
            Integer = 2,
            Decimal = 3,
            DateTime = 4,
            Boolean = 5,
            Uniqueidentifier = 6,
            Long = 7,
        }

        public enum e_ValueType
        {
            [Description("Variable")]
            [XmlEnum("1")] Variable = 1,
            [Description("Static")]
            [XmlEnum("2")] Static = 2,
            [Description("Control")]
            [XmlEnum("3")] Control = 3,
            [Description("Parameter")]
            [XmlEnum("4")] Parameter = 4,
            [Description("SysParameter")]
            [XmlEnum("6")] SysParameter = 6,
        }
    }
}
