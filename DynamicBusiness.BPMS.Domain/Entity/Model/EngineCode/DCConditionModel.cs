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
    public class DCConditionModel : DCBaseModel
    {
        public DCConditionModel() { }
        public DCConditionModel(string id, string name, string shapeid, string parentShapeId, bool? isOutputYes, List<DCRowConditionModel> rows, bool isFirst, string funcName)
            : base(id, name, e_ActionType.Condition, parentShapeId, shapeid, isOutputYes, isFirst, funcName)
        {
            this.Rows = rows;
            this.EvaluateType = e_EvaluateType.And;
            //if there is now row condition it add a row condition
            if (this.Rows == null || this.Rows.Count == 0)
                this.Rows = new List<DCRowConditionModel>() {
                new DCRowConditionModel(){
                    FirstConditionType=DCBaseModel.e_ValueType.Static,
                    FirstConditionValue="",
                    SecondConditionType=DCBaseModel.e_ValueType.Static,
                    SecondConditionValue="",
                    OperationType=DCRowConditionModel.e_OperationType.Equal },
                };
        }

        [DataMember]
        public List<DCRowConditionModel> Rows { get; set; }

        public override object FillData(XElement xElement)
        {
            base.FillData(xElement);
            this.EvaluateType = (DCConditionModel.e_EvaluateType)xElement.GetValue(nameof(DCConditionModel.EvaluateType)).ToIntObj();
            this.Rows = (from c in xElement.Element(nameof(DCConditionModel.Rows)).Elements(nameof(DCRowConditionModel))
                         select new DCRowConditionModel()
                         {
                             FirstConditionType = (DCBaseModel.e_ValueType)c.GetValue(nameof(DCRowConditionModel.FirstConditionType)).ToIntObj(),
                             SecondConditionType = (DCBaseModel.e_ValueType)c.GetValue(nameof(DCRowConditionModel.SecondConditionType)).ToIntObj(),
                             FirstConditionValue = c.GetValue(nameof(DCRowConditionModel.FirstConditionValue)),
                             SecondConditionValue = c.GetValue(nameof(DCRowConditionModel.SecondConditionValue)),
                             OperationType = (DCRowConditionModel.e_OperationType)c.GetValue(nameof(DCRowConditionModel.OperationType)).ToIntObj(),
                         }).ToList();
            return this;
        }

        public override XElement ToXmlElement()
        {
            return new XElement(nameof(DCConditionModel),
                     base.ToXmlElementArray(),
                     new XElement(nameof(DCConditionModel.EvaluateType), (int)this.EvaluateType),
                     new XElement(nameof(DCConditionModel.Rows),
                     from c in this.Rows
                     select new XElement(nameof(DCRowConditionModel),
                         new XElement(nameof(DCRowConditionModel.FirstConditionType), (int)c.FirstConditionType),
                         new XElement(nameof(DCRowConditionModel.SecondConditionType), (int)c.SecondConditionType),
                         new XElement(nameof(DCRowConditionModel.FirstConditionValue), c.FirstConditionValue),
                         new XElement(nameof(DCRowConditionModel.SecondConditionValue), c.SecondConditionValue),
                         new XElement(nameof(DCRowConditionModel.OperationType), (int)c.OperationType)
                         ))
                     );
        }

        public override bool Execute(ICodeBase codeBase)
        {
            bool result = true;
            foreach (var item in this.Rows)
            {
                DCBaseModel.e_ConvertType e_FirstSideConvert = e_ConvertType.String;
                switch (item.FirstConditionType)
                {
                    case e_ValueType.Variable:
                        e_FirstSideConvert = DCBaseModel.GetVariableConvertType(item.FirstConditionValue, codeBase.GetProcessID, codeBase.GetApplicationPageID, codeBase.GetUnitOfWork);
                        break;
                }
                object firstValue = DCBaseModel.GetValue(codeBase, item.FirstConditionValue, item.FirstConditionType, e_FirstSideConvert);
                object SecondValue = DCBaseModel.GetValue(codeBase, item.SecondConditionValue, item.SecondConditionType, e_FirstSideConvert);

                switch (e_FirstSideConvert)
                {
                    case e_ConvertType.String:
                        switch (item.OperationType)
                        {
                            case DCRowConditionModel.e_OperationType.Equal:
                                result = firstValue.ToStringObjNull() == SecondValue.ToStringObjNull();
                                break;
                            case DCRowConditionModel.e_OperationType.BiggerAndEqualThan:
                                result = (firstValue.ToStringObjNull() == SecondValue.ToStringObjNull()) || (firstValue != null && firstValue.ToStringObjNull().CompareTo(SecondValue.ToStringObjNull()) >= 0);
                                break;
                            case DCRowConditionModel.e_OperationType.LowerAndEqualThan:
                                result = firstValue == null || firstValue.ToStringObjNull().CompareTo(SecondValue.ToStringObjNull()) <= 0;
                                break;
                            case DCRowConditionModel.e_OperationType.BiggerThan:

                                result = firstValue != null && firstValue.ToStringObjNull().CompareTo(SecondValue.ToStringObjNull()) == 1;
                                break;
                            case DCRowConditionModel.e_OperationType.LowerThan:
                                result = (firstValue == null && SecondValue != null) || (firstValue != null && firstValue.ToStringObjNull().CompareTo(SecondValue.ToStringObjNull()) == -1);
                                break;
                            case DCRowConditionModel.e_OperationType.NotEqual:
                                result = firstValue.ToStringObjNull() != SecondValue.ToStringObjNull();
                                break;
                            default: break;
                        }
                        break;
                    case e_ConvertType.Integer:
                        switch (item.OperationType)
                        {
                            case DCRowConditionModel.e_OperationType.Equal:
                                result = firstValue.ToIntObjNull() == SecondValue.ToIntObjNull();
                                break;
                            case DCRowConditionModel.e_OperationType.BiggerAndEqualThan:
                                result = firstValue.ToIntObjNull() >= SecondValue.ToIntObjNull();
                                break;
                            case DCRowConditionModel.e_OperationType.LowerAndEqualThan:
                                result = firstValue.ToIntObjNull() <= SecondValue.ToIntObjNull();
                                break;
                            case DCRowConditionModel.e_OperationType.BiggerThan:
                                result = firstValue.ToIntObjNull() > SecondValue.ToIntObjNull();
                                break;
                            case DCRowConditionModel.e_OperationType.LowerThan:
                                result = firstValue.ToIntObjNull() < SecondValue.ToIntObjNull();
                                break;
                            case DCRowConditionModel.e_OperationType.NotEqual:
                                result = firstValue.ToIntObjNull() != SecondValue.ToIntObjNull();
                                break;
                            default: break;
                        }
                        break;
                    case e_ConvertType.Decimal:
                        switch (item.OperationType)
                        {
                            case DCRowConditionModel.e_OperationType.Equal:
                                result = firstValue.ToDecimalObjNull() == SecondValue.ToDecimalObjNull();
                                break;
                            case DCRowConditionModel.e_OperationType.BiggerAndEqualThan:
                                result = firstValue.ToDecimalObjNull() >= SecondValue.ToDecimalObjNull();
                                break;
                            case DCRowConditionModel.e_OperationType.LowerAndEqualThan:
                                result = firstValue.ToDecimalObjNull() <= SecondValue.ToDecimalObjNull();
                                break;
                            case DCRowConditionModel.e_OperationType.BiggerThan:
                                result = firstValue.ToDecimalObjNull() > SecondValue.ToDecimalObjNull();
                                break;
                            case DCRowConditionModel.e_OperationType.LowerThan:
                                result = firstValue.ToDecimalObjNull() < SecondValue.ToDecimalObjNull();
                                break;
                            case DCRowConditionModel.e_OperationType.NotEqual:
                                result = firstValue.ToDecimalObjNull() != SecondValue.ToDecimalObjNull();
                                break;
                            default: break;
                        }
                        break;
                    case e_ConvertType.DateTime:
                        switch (item.OperationType)
                        {
                            case DCRowConditionModel.e_OperationType.Equal:
                                result = firstValue.ToDateTimeObjNull() == SecondValue.ToDateTimeObjNull();
                                break;
                            case DCRowConditionModel.e_OperationType.BiggerAndEqualThan:
                                result = firstValue.ToDateTimeObjNull() >= SecondValue.ToDateTimeObjNull();
                                break;
                            case DCRowConditionModel.e_OperationType.LowerAndEqualThan:
                                result = firstValue.ToDateTimeObjNull() <= SecondValue.ToDateTimeObjNull();
                                break;
                            case DCRowConditionModel.e_OperationType.BiggerThan:
                                result = firstValue.ToDateTimeObjNull() > SecondValue.ToDateTimeObjNull();
                                break;
                            case DCRowConditionModel.e_OperationType.LowerThan:
                                result = firstValue.ToDateTimeObjNull() < SecondValue.ToDateTimeObjNull();
                                break;
                            case DCRowConditionModel.e_OperationType.NotEqual:
                                result = firstValue.ToDateTimeObjNull() != SecondValue.ToDateTimeObjNull();
                                break;
                            default: break;
                        }
                        break;
                    case e_ConvertType.Boolean:
                        switch (item.OperationType)
                        {
                            case DCRowConditionModel.e_OperationType.Equal:
                                result = firstValue.ToBoolObjNull() == SecondValue.ToBoolObjNull();
                                break;
                            case DCRowConditionModel.e_OperationType.NotEqual:
                                result = firstValue.ToBoolObjNull() != SecondValue.ToBoolObjNull();
                                break;
                            default: result = false; break;
                        }
                        break;
                    case e_ConvertType.Uniqueidentifier:
                        switch (item.OperationType)
                        {
                            case DCRowConditionModel.e_OperationType.Equal:
                                result = firstValue.ToGuidObjNull() == SecondValue.ToGuidObjNull();
                                break;
                            case DCRowConditionModel.e_OperationType.NotEqual:
                                result = firstValue.ToGuidObjNull() != SecondValue.ToGuidObjNull();
                                break;
                            default: result = false; break;
                        }
                        break;
                    case e_ConvertType.Long:
                        switch (item.OperationType)
                        {
                            case DCRowConditionModel.e_OperationType.Equal:
                                result = firstValue.ToLongObjNull() == SecondValue.ToLongObjNull();
                                break;
                            case DCRowConditionModel.e_OperationType.BiggerAndEqualThan:
                                result = firstValue.ToLongObjNull() >= SecondValue.ToLongObjNull();
                                break;
                            case DCRowConditionModel.e_OperationType.LowerAndEqualThan:
                                result = firstValue.ToLongObjNull() <= SecondValue.ToLongObjNull();
                                break;
                            case DCRowConditionModel.e_OperationType.BiggerThan:
                                result = firstValue.ToLongObjNull() > SecondValue.ToLongObjNull();
                                break;
                            case DCRowConditionModel.e_OperationType.LowerThan:
                                result = firstValue.ToLongObjNull() < SecondValue.ToLongObjNull();
                                break;
                            case DCRowConditionModel.e_OperationType.NotEqual:
                                result = firstValue.ToLongObjNull() != SecondValue.ToLongObjNull();
                                break;
                            default: break;
                        }
                        break;
                }
                if (this.EvaluateType == e_EvaluateType.And && !result)
                {
                    break;
                }
                if (this.EvaluateType == e_EvaluateType.Or && result)
                {
                    break;
                }
            }
            return result;
        }

        [DataMember]
        public e_EvaluateType EvaluateType { get; set; }

        public enum e_EvaluateType
        {
            [Description("And")]
            And = 1,
            [Description("Or")]
            Or = 2
        }
    }

    /// <summary>
    /// each row of condition 
    /// </summary>
    [DataContract]
    public class DCRowConditionModel
    {
        [DataMember]
        public DCBaseModel.e_ValueType FirstConditionType { get; set; }
        [DataMember]
        public string FirstConditionValue { get; set; }
        [DataMember]
        public DCBaseModel.e_ValueType SecondConditionType { get; set; }
        [DataMember]
        public string SecondConditionValue { get; set; }
        [DataMember]
        public e_OperationType OperationType { get; set; }

        public enum e_OperationType
        {
            [Description("Equal")]
            Equal = 1,
            [Description("BiggerAndEqualThan")]
            BiggerAndEqualThan = 2,
            [Description("LowerAndEqualThan")]
            LowerAndEqualThan = 3,
            [Description("BiggerThan")]
            BiggerThan = 4,
            [Description("LowerThan")]
            LowerThan = 5,
            [Description("NotEqual")]
            NotEqual = 6,
            //[Description("ISNULL")]
            //ISNULL = 7,
        }

        public string GetOperationType()
        {
            switch (this.OperationType)
            {
                case e_OperationType.Equal: return " == ";
                case e_OperationType.BiggerAndEqualThan: return " >= ";
                case e_OperationType.LowerAndEqualThan: return " <= ";
                case e_OperationType.BiggerThan: return " > ";
                case e_OperationType.LowerThan: return " < ";
                case e_OperationType.NotEqual: return " != ";
                //case e_OperationType.ISNULL: return " == null";
                default: return "";
            }
        }
    }
}
