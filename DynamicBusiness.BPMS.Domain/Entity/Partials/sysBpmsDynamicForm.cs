using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    [Serializable]
    [KnownType(typeof(sysBpmsDynamicForm))]
    public partial class sysBpmsDynamicForm
    {
        public sysBpmsDynamicForm Update(Guid? processID, Guid? applicationPageID, string name, int version, bool? showInOverview)
        {
            this.ProcessId = processID;
            this.ApplicationPageID = applicationPageID;
            this.Name = name;
            this.Version = version;
            this.ShowInOverview = showInOverview;
            return this;
        }

        public sysBpmsDynamicForm Update(DynamicFormConfigXmlModel dynamicFormConfigXmlModel)
        {
            this.ConfigXML = dynamicFormConfigXmlModel.BuildXml();
            return this;
        }

        public ResultOperation Update(string designJson)
        {
            this.DesignJson = designJson;
            ResultOperation resultOperation = new ResultOperation();
            if (this.DesignJson != null && this.DesignJson.LastIndexOf("\"subtype\":\"threadTaskDescription\"") != this.DesignJson.IndexOf("\"subtype\":\"threadTaskDescription\""))
            {
                resultOperation.AddError(LangUtility.Get("DescriptionError.Text", nameof(sysBpmsDynamicForm)));
            }
            return resultOperation;
        }

        public void UpdateOnExitFormCode(string onExitFormCode)
        {
            this.OnExitFormCode = onExitFormCode;
        }

        public void UpdateOnEntryFormCode(string onEntryFormCode)
        {
            this.OnEntryFormCode = onEntryFormCode;
        }
         
        public object FindControl(string controlID)
        {
            return ElementBase.GetElement(this.GetControls().FirstOrDefault(c => c["id"].ToStringObj() == controlID), null, this.ID, false);
        }

        public List<JObject> GetControls()
        {
            return JObject.Parse(this.DesignJson)?["rows"].SelectMany(c =>
              {
                  if (((JObject)c)["columns"] != null)
                      return ((JObject)c)["columns"].SelectMany(f => (((JObject)f)["children"].Select(g => (JObject)g)));
                  else
                      return ((JObject)c)["cards"].SelectMany(f => (((JObject)f)["rows"].SelectMany(g => ((JObject)g)["columns"].SelectMany(h => (((JObject)h)["children"].Select(a => (JObject)a))))));
              }
            ).ToList();
        }
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public DynamicFormConfigXmlModel ConfigXmlModel
        {
            get
            {
                return this.ConfigXML.ToStringObj().ParseXML<DynamicFormConfigXmlModel>() ?? new DynamicFormConfigXmlModel();
            }
        }
         
        public sysBpmsDynamicForm Clone()
        {
            return new sysBpmsDynamicForm
            {
                ID = this.ID,
                ProcessId = this.ProcessId,
                ApplicationPageID = this.ApplicationPageID,
                Name = this.Name,
                DesignJson = this.DesignJson,
                OnExitFormCode = this.OnExitFormCode,
                OnEntryFormCode = this.OnEntryFormCode,
                Version = this.Version,
                ConfigXML = this.ConfigXML,
                ShowInOverview = this.ShowInOverview,
                SourceCode = this.SourceCode,
                CreatedBy = this.CreatedBy,
                CreatedDate = this.CreatedDate,
                UpdatedBy = this.UpdatedBy,
                UpdatedDate = this.UpdatedDate, 
            };
        }

    }
}
