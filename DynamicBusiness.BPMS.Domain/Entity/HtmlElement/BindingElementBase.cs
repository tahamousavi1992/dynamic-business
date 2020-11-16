using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    [DataContract]
    public class BindingElementBase : ElementBase
    {
        public BindingElementBase() { }
        public BindingElementBase(JObject obj, HtmlElementHelperModel _helper, Guid dynamicFormId, bool isFormReadOnly) : base(obj, _helper, dynamicFormId)
        {
            this.Fill = DomainUtility.toString(obj["fillBinding"]);
            this.Map = DomainUtility.toString(obj["mapBinding"]);
            this.Parameter = obj["parameter"].ToStringObj();
            this.IsReadonly = isFormReadOnly ? true : (obj["readOnly"] != null ? ((bool)obj["readOnly"]) : (bool?)null);
            this.ValidationGroup = string.IsNullOrWhiteSpace(obj["validationGroup"].ToStringObj()) ? "nextAction" : obj["validationGroup"].ToStringObj().Trim();
        }
        [DataMember]
        public string Parameter { get; set; }
        [DataMember]
        public string Fill { get; set; }
        [DataMember]
        public string Map { get; set; }
        [DataMember]
        public object Value { get; set; }
        [DataMember]
        public bool? IsReadonly { get; set; }
        [DataMember]
        public string ValidationGroup { get; set; }
        [DataMember]
        public List<DependentParameterModel> ListDependentParameterModel
        {
            get
            {
                return this.Parameter.ToStringObj().Split(',').Where(c => !string.IsNullOrWhiteSpace(c)).Select(c => new DependentParameterModel(c.Split(':')[0], c.Split(':')[1], c.Split(':')[2].ToLower() == "true")).ToList();
            }
            private set { }
        }
        public virtual void SetValue(object value)
        {
            this.Value = value;
        }

        public virtual void IsValid(ResultOperation resultOperation)
        {

        }
        public virtual void FillData(List<QueryModel> listFormQueryModel = null)
        {

        }
        public bool IsInForm(List<QueryModel> listFormQueryModel)
        {
            return listFormQueryModel.Any(c => c.Key == this.Id || (c.Key == this.Id + "[]" /*for files */)) && this.IsReadonly != true;
        }
        //It is used to bind a list Item like datagrid,dropdown and ... using passing parameter not with fillList variable.
        public virtual void BindDataSource(object data)
        {

        }
    }
}
