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
    public class DCSetVariableModel : DCBaseModel
    {
        public DCSetVariableModel() { }
        public DCSetVariableModel(string id, string shapeid, string parentShapeId, List<DCRowSetVariableModel> rows, bool? isOutputYes, bool isFirst, string funcName)
            : base(id, string.Empty, e_ActionType.SetVariable, parentShapeId, shapeid, isOutputYes, isFirst, funcName)
        {
            this.Rows = rows;
            //if there is now row condition it add a row condition
            if (this.Rows == null || this.Rows.Count == 0)
                this.Rows = new List<DCRowSetVariableModel>() {
                new DCRowSetVariableModel(){
                     Value=string.Empty,
                     ValueType=e_ValueType.Static,
                     VaribleName=string.Empty
                     },
                };
        }

        [DataMember]
        public List<DCRowSetVariableModel> Rows { get; set; }

        public override bool Execute(ICodeBase codeBase)
        {
            foreach (var item in this.Rows)
            {
                codeBase.VariableHelper.Set(item.VaribleName, DCBaseModel.GetValue(codeBase, item.Value, item.ValueType, DCBaseModel.e_ConvertType.String));
            }
            return true;
        }
        public override object FillData(XElement xElement)
        {
            base.FillData(xElement);
            this.ActionType = (DCBaseModel.e_ActionType)xElement.Element(nameof(DCBaseModel.ActionType)).Value.ToIntObj();
            this.Rows = (from c in xElement.Element(nameof(DCSetVariableModel.Rows)).Elements(nameof(DCRowSetVariableModel))
                         select new DCRowSetVariableModel()
                         {
                             ValueType = (DCBaseModel.e_ValueType)c.Element(nameof(DCRowSetVariableModel.ValueType)).Value.ToIntObj(),
                             Value = c.Element(nameof(DCRowSetVariableModel.Value)).Value,
                             VaribleName = c.Element(nameof(DCRowSetVariableModel.VaribleName)).Value,
                         }).ToList();
            return this;
        }
        public override XElement ToXmlElement()
        {
            return new XElement(nameof(DCSetVariableModel),
                     base.ToXmlElementArray(),
                     new XElement(nameof(Rows),
                     from c in this.Rows
                     select new XElement(nameof(DCRowSetVariableModel),
                         new XElement(nameof(DCRowSetVariableModel.Value), c.Value),
                         new XElement(nameof(DCRowSetVariableModel.ValueType), (int)c.ValueType),
                         new XElement(nameof(DCRowSetVariableModel.VaribleName), c.VaribleName)
                         )));
        }
    }
    /// <summary>
    /// each row of SetVariable 
    /// </summary>
    [DataContract]
    public class DCRowSetVariableModel
    {
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
    }
}
