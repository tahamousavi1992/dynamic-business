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
    [KnownType(typeof(FormHtml))]
    public class FormHtml : ElementBase
    {
        public FormHtml()
        {

        }
        /// <summary>
        /// this method set HtmlElement to ColumnHtml and RowHtml and FormHtml
        /// </summary>
        /// <param name="_helper">if set null it does not fill element values.</param>
        public FormHtml(JObject obj, HtmlElementHelperModel _helper, Guid dynamicFormId, bool isFormReadOnly) : base(obj, _helper, dynamicFormId)
        {
            if (dynamicFormId != Guid.Empty && _helper?.UnitOfWork != null && this.Helper?.FormAction != HtmlElementHelperModel.e_FormAction.FillMode)
            {
                sysBpmsDynamicForm dynamicForm = _helper?.UnitOfWork.Repository<IDynamicFormRepository>().GetInfo(dynamicFormId);
                if (dynamicForm != null)
                {
                    //If It is ApplicationPage,It must set ApplicationPageID To current ApplicationPageID for DataManageHelper and DocumentEngine 
                    //to retrieve variables for current form
                    Guid? parentAppPageID = _helper.DataManageHelper.GetSharedModel().CurrentApplicationPageID;
                    if (dynamicForm.ApplicationPageID.HasValue)
                    {
                        _helper.DataManageHelper.SetApplicationPageID(dynamicForm.ApplicationPageID.Value);
                        _helper.DocumentEngine.SetApplicationPageID(dynamicForm.ApplicationPageID.Value);
                    }

                    isFormReadOnly = !isFormReadOnly ? (obj["readOnly"] != null ? ((bool)obj["readOnly"]) : false) : true;
                    //convert form xml code to json object
                    JObject newObj = JObject.Parse(dynamicForm.DesignJson);
                    //if json object has a control with type = CONTENT
                    if (newObj != null && newObj["type"].ToString() == "CONTENT")
                    {
                        this.ContentHtml = new ContentHtml(newObj, _helper, dynamicFormId, isFormReadOnly);
                    }
                    base.Helper?.AddScript(dynamicForm.ConfigXmlModel.OnLoadFunctionBody);
                    base.Helper?.AddStyleSheet(dynamicForm.ConfigXmlModel.StyleSheetCode);
                    this.IsFormReadOnly = isFormReadOnly;

                    //If It is ApplicationPage,It must restore ApplicationPageID To parent form for DataManageHelper and DocumentEngine
                    if (dynamicForm.ApplicationPageID.HasValue)
                    {
                        _helper.DataManageHelper.SetApplicationPageID(parentAppPageID.Value);
                        _helper.DocumentEngine.SetApplicationPageID(parentAppPageID.Value);
                    }
                }
            }

        }
        [DataMember]
        public bool IsFormReadOnly { get; set; }
        [DataMember]
        public ContentHtml ContentHtml { get; set; }
    }
}
