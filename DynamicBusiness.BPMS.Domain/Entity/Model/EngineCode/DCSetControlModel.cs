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
    public class DCSetControlModel : DCBaseModel
    {
        public DCSetControlModel() { }
        public DCSetControlModel(string id, string shapeid, string parentShapeId, string controlId, e_ValueType valueType, string value, bool? isOutputYes, bool isFirst, string funcName)
            : base(id, string.Empty, e_ActionType.SetControl, parentShapeId, shapeid, isOutputYes, isFirst, funcName)
        {
            this.ControlId = controlId;
            this.ValueType = valueType;
            this.Value = value;
        }
        [DataMember]
        public string ControlId { get; set; }
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
            codeBase.ControlHelper.SetValue(this.ControlId, DCBaseModel.GetValue(codeBase, this.Value, this.ValueType, DCBaseModel.e_ConvertType.String));
            return true;
        }
        public override object FillData(XElement xElement)
        {
            base.FillData(xElement);
            this.ActionType = (DCBaseModel.e_ActionType)xElement.Element(nameof(DCBaseModel.ActionType)).Value.ToIntObj();
            this.ValueType = (DCBaseModel.e_ValueType)xElement.Element(nameof(DCSetControlModel.ValueType)).Value.ToIntObj();
            this.Value = xElement.Element(nameof(DCSetControlModel.Value)).Value;
            this.ControlId = xElement.Element(nameof(DCSetControlModel.ControlId)).Value;
            return this;
        }
        public override XElement ToXmlElement()
        {
            return new XElement(nameof(DCSetControlModel),
                     base.ToXmlElementArray(),
                     new XElement(nameof(DCSetControlModel.Value), this.Value),
                     new XElement(nameof(DCSetControlModel.ValueType), (int)this.ValueType),
                     new XElement(nameof(DCSetControlModel.ControlId), this.ControlId)
                     );
        }
    }
}
