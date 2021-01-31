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
    public class DCSqlFunctionModel : DCBaseModel
    {
        public DCSqlFunctionModel() { }
        public DCSqlFunctionModel(string id, string name, string retVariableName, string query, string shapeid, string parentShapeId, bool? isOutputYes, List<DCRowSqlParameterModel> rows, bool isFirst, string funcName)
            : base(id, name, e_ActionType.SqlFunction, parentShapeId, shapeid, isOutputYes, isFirst, funcName)
        {
            this.Query = query;
            this.RetVariableName = retVariableName;
            this.Rows = rows ?? new List<DCRowSqlParameterModel>();
        }
        [DataMember]
        [Required]
        public string Query { get; set; }
        [DataMember]
        public string RetVariableName { get; set; }
        [DataMember]
        public e_MethodType MethodType { get; set; }
        [DataMember]
        public List<DCRowSqlParameterModel> Rows { get; set; }

        public override object FillData(XElement xElement)
        {
            base.FillData(xElement);
            this.Query = HttpUtility.UrlDecode(xElement.GetValue(nameof(DCSqlFunctionModel.Query)));
            this.RetVariableName = xElement.GetValue(nameof(DCSqlFunctionModel.RetVariableName));
            this.MethodType = (DCSqlFunctionModel.e_MethodType)xElement.GetValue(nameof(DCSqlFunctionModel.MethodType)).ToIntObj();

            this.Rows = (from c in xElement.Element(nameof(DCSqlFunctionModel.Rows)).Elements(nameof(DCRowSqlParameterModel))
                         select new DCRowSqlParameterModel()
                         {
                             Value = c.GetValue(nameof(DCRowSqlParameterModel.Value)).ToStringObj(),
                             Name = c.GetValue(nameof(DCRowSqlParameterModel.Name)).ToStringObj(),
                             ValueType = (DCBaseModel.e_ValueType)c.GetValue(nameof(DCRowSqlParameterModel.ValueType)).ToIntObj(),
                         }).ToList();
            return this;
        }

        public override XElement ToXmlElement()
        {
            return new XElement(nameof(DCSqlFunctionModel),
                     base.ToXmlElementArray(),
                     new XElement(nameof(DCSqlFunctionModel.Query), HttpUtility.UrlEncode(this.Query)),
                     new XElement(nameof(DCSqlFunctionModel.RetVariableName), this.RetVariableName),
                     new XElement(nameof(DCSqlFunctionModel.MethodType), (int)this.MethodType),
                     new XElement(nameof(DCSqlFunctionModel.Rows),
                     from c in this.Rows
                     select new XElement(nameof(DCRowSqlParameterModel),
                         new XElement(nameof(DCRowSqlParameterModel.Value), c.Value),
                         new XElement(nameof(DCRowSqlParameterModel.Name), c.Name),
                         new XElement(nameof(DCRowSqlParameterModel.ValueType), (int)c.ValueType)
                         ))
                     );
        }

        public override bool Execute(ICodeBase codeBase)
        {
            List<QueryModel> parameterList = this.Rows.Select(item => new QueryModel(item.Name, this.GetParameterCode(codeBase, item))).ToList();
            object result = null;
            switch (this.MethodType)
            {
                case e_MethodType.Execute:
                    result = codeBase.QueryHelper.Execute(this.Query, parameterList.ToArray());
                    break;
                case e_MethodType.Scalar:
                    result = codeBase.QueryHelper.ExecuteScalar<object>(this.Query, parameterList.ToArray());
                    break;
                case e_MethodType.DataTable:
                    result = codeBase.QueryHelper.Get(this.Query, parameterList.ToArray());
                    break;
            }
            if (!string.IsNullOrWhiteSpace(this.RetVariableName))
            {
                codeBase.VariableHelper.Set(this.RetVariableName, result);
            }
            return true;
        }

        private object GetParameterCode(ICodeBase codeBase, DCRowSqlParameterModel paraetersModel)
        {
            return paraetersModel.GetRendered(codeBase, e_ConvertType.String);
        }

        public enum e_MethodType
        {
            [Description("Execute")]
            Execute = 1,
            [Description("Scalar")]
            Scalar = 2,
            [Description("DataTable")]
            DataTable = 3,
        }
    }

    /// <summary>
    /// Each row of Parameter 
    /// </summary>
    [DataContract]
    public class DCRowSqlParameterModel
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Value { get; set; }
        [DataMember]
        public DCBaseModel.e_ValueType ValueType { get; set; }

        public object GetRendered(ICodeBase codeBase, DCBaseModel.e_ConvertType e_Convert)
        {
            return DCBaseModel.GetValue(codeBase, this.Value, this.ValueType, e_Convert);
        }

    }
}
