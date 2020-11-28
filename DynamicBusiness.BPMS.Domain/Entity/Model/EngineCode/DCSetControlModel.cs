using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using static DynamicBusiness.BPMS.Domain.DCBaseModel;

namespace DynamicBusiness.BPMS.Domain
{
    [DataContract]
    public class DCSetControlModel : DCBaseModel
    {
        public DCSetControlModel() { }
        public DCSetControlModel(string id, string shapeid, string parentShapeId, List<DCRowSetControlModel> rows, bool? isOutputYes, bool isFirst, string funcName)
            : base(id, string.Empty, e_ActionType.SetControl, parentShapeId, shapeid, isOutputYes, isFirst, funcName)
        {
            this.Rows = rows;
            //if there is now row condition it add a row condition
            if (this.Rows == null || this.Rows.Count == 0)
                this.Rows = new List<DCRowSetControlModel>() {
                new DCRowSetControlModel(){
                     Value=string.Empty,
                     ValueType=e_ValueType.Static,
                     ControlId=string.Empty
                     },
                };
        }
        [DataMember]
        public List<DCRowSetControlModel> Rows { get; set; }
        public override bool Execute(ICodeBase codeBase)
        {
            foreach (var item in this.Rows)
            {
                codeBase.ControlHelper.SetValue(item.ControlId, DCBaseModel.GetValue(codeBase, item.Value, item.ValueType, DCBaseModel.e_ConvertType.String));
            }
            return true;
        }
        public override object FillData(XElement xElement)
        {
            base.FillData(xElement);
            this.ActionType = (DCBaseModel.e_ActionType)xElement.Element(nameof(DCBaseModel.ActionType)).Value.ToIntObj();
            this.Rows = (from c in xElement.Element(nameof(DCSetControlModel.Rows)).Elements(nameof(DCRowSetControlModel))
                         select new DCRowSetControlModel()
                         {
                             ValueType = (DCBaseModel.e_ValueType)c.Element(nameof(DCRowSetControlModel.ValueType)).Value.ToIntObj(),
                             Value = c.Element(nameof(DCRowSetControlModel.Value)).Value,
                             ControlId = c.Element(nameof(DCRowSetControlModel.ControlId)).Value,
                         }).ToList();

            return this;
        }
        public override XElement ToXmlElement()
        {
            return new XElement(nameof(DCSetControlModel),
                     base.ToXmlElementArray(),
                     new XElement(nameof(Rows),
                     from c in this.Rows
                     select new XElement(nameof(DCRowSetControlModel),
                         new XElement(nameof(DCRowSetControlModel.Value), c.Value),
                         new XElement(nameof(DCRowSetControlModel.ValueType), (int)c.ValueType),
                         new XElement(nameof(DCRowSetControlModel.ControlId), c.ControlId)
                         )
                     ));
        }
    }
    /// <summary>
    /// each row of SetControl
    /// </summary>
    [DataContract]
    public class DCRowSetControlModel
    {
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
    }
}
