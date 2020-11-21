using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace DynamicBusiness.BPMS.Domain
{
    [DataContract]
    public class DCSetVariableModel : DCBaseModel
    {
        public DCSetVariableModel() { }
        public DCSetVariableModel(string id, string shapeid, string parentShapeId, string varibleName, e_ValueType valueType, string value, bool? isOutputYes, bool isFirst, string funcName)
            : base(id, string.Empty, e_ActionType.SetVariable, parentShapeId, shapeid, isOutputYes, isFirst, funcName)
        {
            this.VaribleName = varibleName;
            this.ValueType = valueType;
            this.Value = value;
        }
        [DataMember]
        public string VaribleName { get; set; }
        [DataMember]
        public e_ValueType ValueType { get; set; }
        /// <summary>
        /// if value type is method ,value is a base64 string of method xml design because it can not accept second  <<![CDATA[]]>
        /// in xml due to master path used it first.
        /// </summary>
        [DataMember]
        public string Value { get; set; }

        public override bool Execute(ICodeBase codeBase)
        {
            codeBase.VariableHelper.Set(this.VaribleName, DCBaseModel.GetValue(codeBase, this.Value, this.ValueType, DCBaseModel.e_ConvertType.String));
            return true;
        }
        public override object FillData(XElement xElement)
        {
            base.FillData(xElement);
            this.ActionType = (DCBaseModel.e_ActionType)xElement.Element(nameof(DCBaseModel.ActionType)).Value.ToIntObj();
            this.ValueType = (DCBaseModel.e_ValueType)xElement.Element(nameof(DCSetVariableModel.ValueType)).Value.ToIntObj();
            this.Value = xElement.Element(nameof(DCSetVariableModel.Value)).Value;
            this.VaribleName = xElement.Element(nameof(DCSetVariableModel.VaribleName)).Value;
            return this;
        }
        public override XElement ToXmlElement()
        {
            return new XElement(nameof(DCSetVariableModel),
                     base.ToXmlElementArray(),
                     new XElement(nameof(DCSetVariableModel.Value), this.Value),
                     new XElement(nameof(DCSetVariableModel.ValueType), (int)this.ValueType),
                     new XElement(nameof(DCSetVariableModel.VaribleName), this.VaribleName)
                     );
        }
    }
}
