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
    public class ContentHtml : ElementBase
    {
        public ContentHtml()
        {

        }

        public List<T> FindControlByType<T>()
        {
            return this.Rows.SelectMany(r => (r is RowHtml ? ((RowHtml)r).Columns : ((AccordionHtml)r).GetListColumn().ToList()).Select(c =>
            {
                return c.children.Select(child => child is ContentHtml ? ((ContentHtml)child).FindControlByType<T>() : child).FirstOrDefault(f => f is T);
            })).Where(c => c != null).Select(c => (T)c).ToList();
        }

        public object FindControlByID(string controlID)
        {
            return this.Rows.SelectMany(r => (r is RowHtml ? ((RowHtml)r).Columns : ((AccordionHtml)r).GetListColumn().ToList()).Select(c =>
                    {
                        return c.children.Select(child => child is ContentHtml ? ((ContentHtml)child).FindControlByID(controlID) : child).FirstOrDefault(f => ((ElementBase)f).Id == controlID);
                    })).FirstOrDefault(c => c != null);
        }

        public List<object> FindDependentControls()
        {
            return this.Rows.SelectMany(r =>
            (r is RowHtml ? ((RowHtml)r).Columns : ((AccordionHtml)r).GetListColumn()).SelectMany(c =>
            {
                return c.children.Where(child => (!string.IsNullOrWhiteSpace(DomainUtility.GetPropValue(child, nameof(BindingElementBase.Parameter)).ToStringObj())) || child is ContentHtml).SelectMany(child => child is ContentHtml ? ((ContentHtml)child).FindDependentControls() : new List<object>() { child });
            })).ToList();
        }

        public void CheckAllVisibility(FormModel formModel)
        {
            if (base.Helper?.FormAction != HtmlElementHelperModel.e_FormAction.FillMode)
            {
                this.Rows.Where(c => ((ElementBase)c).CheckVisibility(formModel)).ToList().ForEach(r => (r is RowHtml ? ((RowHtml)r).Columns : ((AccordionHtml)r).GetListColumn()).ForEach(c =>
               {
                   c.children.ForEach((child) =>
                   {
                       if (child is FormHtml)
                       {
                           if (((ElementBase)child).CheckVisibility(formModel))
                           {
                               ((FormHtml)child).ContentHtml?.CheckAllVisibility(formModel);
                           }
                       }
                       else
                        if (child is ContentHtml)
                           ((ContentHtml)child).CheckAllVisibility(formModel);
                       else
                           ((ElementBase)child).CheckVisibility(formModel);
                   });
               }));
            }
        }

        /// <summary>
        /// this method set HtmlElement to ColumnHtml and RowHtml and ContentHtml
        /// </summary>
        /// <param name="_helper">if set null it does not fill element values.</param>
        public ContentHtml(JObject obj, HtmlElementHelperModel _helper, Guid dynamicFormId, bool isFormReadOnly) : base(obj, _helper, dynamicFormId)
        {
            this.Rows = obj["rows"].Select(c => ElementBase.GetElement((JObject)c, _helper, dynamicFormId, isFormReadOnly)).ToList();
        }

        [DataMember]
        //public List<object> rows { get; private set; }
        public List<object> Rows { get; set; }

        public List<RowHtml> GetRowHtmls { get { return this.Rows.SelectMany(c => c is RowHtml ? new List<RowHtml>() { (RowHtml)c } : ((AccordionHtml)c).Cards.SelectMany(d => d.Rows).ToList()).ToList(); } }
        /// <summary>
        /// it is used when we get data from web api and it will return jobject instead of object.
        /// </summary>
        public void ConvertChildrenToObject()
        {
            for (int i = 0; i < this.Rows.Count; i++)
            {
                this.Rows[i] = ElementBase.JObjectTOObject((JObject)this.Rows[i]);
            }
            this.Rows.ForEach(c =>
            {
                if (c is RowHtml)
                    ((RowHtml)c).Columns.ForEach(d => d.children = d.children.Select(f => ElementBase.JObjectTOObject((JObject)f)).ToList());
                else
                    ((AccordionHtml)c).Cards.ForEach(b => b.Rows.ForEach(k => k.Columns.ForEach(d => d.children = d.children.Select(f => ElementBase.JObjectTOObject((JObject)f)).ToList())));
            });
        }
    }
}
