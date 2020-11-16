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
    public class DCExpressionModel : DCBaseModel
    {
        public DCExpressionModel() { }
        public DCExpressionModel(string id, string name, string shapeid, string parentShapeId, string expressionCode, bool? isOutputYes, bool isFirst, string funcName)
            : base(id, name, e_ActionType.Expression, parentShapeId, shapeid, isOutputYes, isFirst, funcName)
        {
            this.ExpressionCode = expressionCode;
            this.Assemblies = string.Empty;
        }

        [DataMember]
        public string ExpressionCode { get; set; }
        [DataMember]
        public string Assemblies { get; set; }
        public override object FillData(XElement xElement)
        {
            base.FillData(xElement);
            this.ExpressionCode = xElement.GetValue(nameof(DCExpressionModel.ExpressionCode));
            this.Assemblies = xElement.GetValue(nameof(DCExpressionModel.Assemblies));
            return this;
        }
        public override XElement ToXmlElement()
        {
            return new XElement(nameof(DCExpressionModel),
                     base.ToXmlElementArray(),
                     new XElement(nameof(DCExpressionModel.ExpressionCode), this.ExpressionCode),
                     new XElement(nameof(DCExpressionModel.Assemblies), this.Assemblies)
                     );
        }
        public override string GetRenderedCode(Guid? processId, Guid? applicationPageId, IUnitOfWork unitOfWork) { return this.ExpressionCode; }
    }

}
