using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;

namespace DynamicBusiness.BPMS.Domain
{
    [DataContract]
    public class DCWebServiceModel : DCBaseModel
    {
        public DCWebServiceModel() { }
        public DCWebServiceModel(string id, string name, string retVariableName, string url, string shapeid, string parentShapeId, bool? isOutputYes, List<DCRowParameterModel> rows, bool isFirst, string funcName)
            : base(id, name, e_ActionType.WebService, parentShapeId, shapeid, isOutputYes, isFirst, funcName)
        {
            this.Url = url;
            this.RetVariableName = retVariableName;
            this.Rows = rows ?? new List<DCRowParameterModel>();
        }
        [DataMember]
        [Required]
        public string Url { get; set; }
        [DataMember]
        public string RetVariableName { get; set; }
        [DataMember]
        public e_ContentType ContentType { get; set; }
        [DataMember]
        public e_MethodType MethodType { get; set; }
        [DataMember]
        public List<DCRowParameterModel> Rows { get; set; }

        public override object FillData(XElement xElement)
        {
            base.FillData(xElement);
            this.Url = HttpUtility.UrlDecode(xElement.GetValue(nameof(DCWebServiceModel.Url)));
            this.RetVariableName = xElement.GetValue(nameof(DCWebServiceModel.RetVariableName));
            this.ContentType = (DCWebServiceModel.e_ContentType)xElement.GetValue(nameof(DCWebServiceModel.ContentType)).ToIntObj();
            this.MethodType = (DCWebServiceModel.e_MethodType)xElement.GetValue(nameof(DCWebServiceModel.MethodType)).ToIntObj();

            this.Rows = (from c in xElement.Element(nameof(DCWebServiceModel.Rows)).Elements(nameof(DCRowParameterModel))
                         select new DCRowParameterModel()
                         {
                             Value = c.GetValue(nameof(DCRowParameterModel.Value)).ToStringObj(),
                             Name = c.GetValue(nameof(DCRowParameterModel.Name)).ToStringObj(),
                             ValueType = (DCBaseModel.e_ValueType)c.GetValue(nameof(DCRowParameterModel.ValueType)).ToIntObj(),
                             Type = (DCRowParameterModel.e_Type)c.GetValue(nameof(DCRowParameterModel.Type)).ToIntObj(),
                         }).ToList();
            return this;
        }

        public override XElement ToXmlElement()
        {
            return new XElement(nameof(DCWebServiceModel),
                     base.ToXmlElementArray(),
                     new XElement(nameof(DCWebServiceModel.Url), HttpUtility.UrlEncode(this.Url)),
                     new XElement(nameof(DCWebServiceModel.RetVariableName), this.RetVariableName),
                     new XElement(nameof(DCWebServiceModel.ContentType), (int)this.ContentType),
                     new XElement(nameof(DCWebServiceModel.MethodType), (int)this.MethodType),
                     new XElement(nameof(DCWebServiceModel.Rows),
                     from c in this.Rows
                     select new XElement(nameof(DCRowParameterModel),
                         new XElement(nameof(DCRowParameterModel.Value), c.Value),
                         new XElement(nameof(DCRowParameterModel.Name), c.Name),
                         new XElement(nameof(DCRowParameterModel.ValueType), (int)c.ValueType),
                         new XElement(nameof(DCRowParameterModel.Type), (int)c.Type)
                         ))
                     );
        }

        public override bool Execute(ICodeBase codeBase)
        {
            List<QueryModel> parameterList = this.Rows.Where(c => c.Type == DCRowParameterModel.e_Type.Parameter).Select(item => new QueryModel(item.Name, this.GetParameterCode(codeBase, item))).ToList();
            List<QueryModel> headerList = this.Rows.Where(c => c.Type == DCRowParameterModel.e_Type.Header).Select(item => new QueryModel(item.Name, this.GetParameterCode(codeBase, item))).ToList();

            object result = null;
            switch (this.MethodType)
            {
                case e_MethodType.Get:
                    result = codeBase.WebServiceHelper.Get(this.Url, headerList, parameterList.ToArray());
                    break;
                case e_MethodType.Post:
                    result = codeBase.WebServiceHelper.Post(this.Url, this.ContentType.GetDescription(), headerList, parameterList.ToArray());
                    break;
                case e_MethodType.Put:
                    result = codeBase.WebServiceHelper.Put(this.Url, this.ContentType.GetDescription(), headerList, parameterList.ToArray());
                    break;
            }
            if (!string.IsNullOrWhiteSpace(this.RetVariableName))
            {
                codeBase.VariableHelper.Set(this.RetVariableName, result);
            }
            return true;
        }

        private object GetParameterCode(ICodeBase codeBase, DCRowParameterModel paraetersModel)
        {
            return paraetersModel.GetRendered(codeBase, e_ConvertType.String);
        }

        public enum e_ContentType
        {
            [Description("application/json")]
            Json = 1,
            [Description("application/x-www-form-urlencoded")]
            UrlEncoded = 2,
        }
        public enum e_MethodType
        {
            [Description("Get")]
            Get = 1,
            [Description("Post")]
            Post = 2,
            [Description("Put")]
            Put = 3,
        }
    }

    /// <summary>
    /// Each row of Parameter 
    /// </summary>
    [DataContract]
    public class DCRowParameterModel
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Value { get; set; }
        [DataMember]
        public DCBaseModel.e_ValueType ValueType { get; set; }
        [DataMember]
        public e_Type Type { get; set; }

        public object GetRendered(ICodeBase codeBase, DCBaseModel.e_ConvertType e_Convert)
        {
            return DCBaseModel.GetValue(codeBase, this.Value, this.ValueType, e_Convert);
        }

        public enum e_Type
        {
            [Description("Parameter")]
            Parameter = 0,
            [Description("Header")]
            Header = 1,
        }
    }
}
