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
        }
        public virtual string GetRenderedCode(Guid? processId, Guid? applicationPageId, IUnitOfWork unitOfWork) { return string.Empty; }
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
        /// <summary>
        /// it will wrape code with corespondent convert to statement.
        /// </summary>
        /// <param name="code">is like VariableHelper.GetValue("varPerson.FirstName")</param>
        /// <returns>wraped code with corespondent convert to statement.</returns>
        public static string WrapCodeWithConvert(string code, e_ConvertType e_Convert, bool allowNull = true)
        {
            switch (e_Convert)
            {
                case e_ConvertType.String:
                    code = (code.ToStringObj().ToLower().Trim() == "null" || code.ToStringObj().ToLower().Trim() == "\"null\"") ? "null" : $"CodeUtility.ToString{(!allowNull ? "Obj" : "")}({code})";
                    break;
                case e_ConvertType.Integer:
                    code = $"CodeUtility.ToInt{(!allowNull ? "Obj" : "")}({code})";
                    break;
                case e_ConvertType.Decimal:
                    code = $"CodeUtility.ToDecimal{(!allowNull ? "Obj" : "")}({code})";
                    break;
                case e_ConvertType.DateTime:
                    code = $"CodeUtility.ToDateTime{(!allowNull ? "Obj" : "")}({code})";
                    break;
                case e_ConvertType.Boolean:
                    code = $"CodeUtility.ToBoolean{(!allowNull ? "Obj" : "")}({code})";
                    break;
                case e_ConvertType.Long:
                    code = $"CodeUtility.ToLong{(!allowNull ? "Obj" : "")}({code})";
                    break;
                case e_ConvertType.Uniqueidentifier:
                    code = $"CodeUtility.ToGuid{(!allowNull ? "Obj" : "")}({code})";
                    break;
            }
            return code;
        }

        public static object GetCodeNew(string code, e_ConvertType e_Convert, bool allowNull = true)
        {
            switch (e_Convert)
            {
                case e_ConvertType.String:
                    return (code.ToStringObj().ToLower().Trim() == "null" || code.ToStringObj().ToLower().Trim() == "\"null\"") ? null : (!allowNull ? CodeUtility.ToStringObj(code) : CodeUtility.ToString(code));
                case e_ConvertType.Integer:
                    return !allowNull ? CodeUtility.ToIntObj(code) : CodeUtility.ToInt(code); ;
                case e_ConvertType.Decimal:
                    return !allowNull ? CodeUtility.ToDecimalObj(code) : CodeUtility.ToDecimal(code);

                case e_ConvertType.DateTime:
                    return !allowNull ? CodeUtility.ToDateTimeObj(code) : CodeUtility.ToDateTime(code); ;

                case e_ConvertType.Boolean:
                    return !allowNull ? CodeUtility.ToBooleanObj(code) : CodeUtility.ToBoolean(code);

                case e_ConvertType.Long:
                    return !allowNull ? CodeUtility.ToLongObj(code) : CodeUtility.ToLong(code);

                case e_ConvertType.Uniqueidentifier:
                    return !allowNull ? CodeUtility.ToGuidObj(code) : CodeUtility.ToGuid(code);
            }
            return code;
        }


        public static string RenderValueType(Guid? processId, Guid? applicationPageId, IUnitOfWork unitOfWork, string value, e_ValueType valueType,
            DCBaseModel.e_ConvertType e_Convert, bool allowNull = true)
        {
            string objectValue = string.Empty;
            switch (valueType)
            {
                case e_ValueType.Static:
                    objectValue = DCBaseModel.WrapCodeWithConvert($"\"{value}\"", e_Convert, allowNull);
                    break;
                case e_ValueType.Control:
                    objectValue = DCBaseModel.WrapCodeWithConvert($"ControlHelper.GetValue(\"{value}\")", e_Convert, allowNull);
                    break;
                case e_ValueType.Parameter:
                    objectValue = DCBaseModel.WrapCodeWithConvert($"UrlHelper.GetParameter(\"{value}\")", e_Convert, allowNull);
                    break;
                case e_ValueType.Variable:
                    objectValue = DCBaseModel.WrapCodeWithConvert($"VariableHelper.GetValue(\"{value}\")", e_Convert, allowNull);
                    break;
                case e_ValueType.SysParameter:
                    objectValue = DCBaseModel.WrapCodeWithConvert($"this.{value}", e_Convert, allowNull);
                    break;
            }
            return objectValue;
        }

        public static object GetValue(string value, e_ValueType valueType,
           DCBaseModel.e_ConvertType e_Convert, bool allowNull = true)
        {
            switch (valueType)
            {
                case e_ValueType.Static:
                    return DCBaseModel.GetCodeNew(value, e_Convert, allowNull);
                case e_ValueType.Control:
                    return DCBaseModel.GetCodeNew($"ControlHelper.GetValue(\"{value}\")", e_Convert, allowNull);
                case e_ValueType.Parameter:
                    return DCBaseModel.GetCodeNew($"UrlHelper.GetParameter(\"{value}\")", e_Convert, allowNull);
                case e_ValueType.Variable:
                    return DCBaseModel.GetCodeNew($"VariableHelper.GetValue(\"{value}\")", e_Convert, allowNull);
                case e_ValueType.SysParameter:
                    return DCBaseModel.GetCodeNew($"this.{value}", e_Convert, allowNull);

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
