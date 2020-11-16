using DynamicBusiness.BPMS.BusinessLogic;
using DynamicBusiness.BPMS.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace DynamicBusiness.BPMS.SharedPresentation
{
    [DataContract]
    public class DynamicFormDTO
    {
        public DynamicFormDTO() { }
        public DynamicFormDTO(sysBpmsDynamicForm dynamicForm)
        {
            if (dynamicForm != null)
            {
                this.ID = dynamicForm.ID;
                this.Name = dynamicForm.Name;
                this.ProcessId = dynamicForm.ProcessId;
                this.ApplicationPageID = dynamicForm.ApplicationPageID;
                this.Version = dynamicForm.Version ?? 1;
                this.DesignJson = dynamicForm.DesignJson;
                this.ConfigXML = dynamicForm.ConfigXML;
                this.OnExitFormCode = dynamicForm.OnExitFormCode;
                this.OnEntryFormCode = dynamicForm.OnEntryFormCode;
                this.ShowInOverview = dynamicForm.ShowInOverview;
                this.CreatedBy = dynamicForm.CreatedBy;
                this.CreatedDate = dynamicForm.CreatedDate;
                this.UpdatedBy = dynamicForm.UpdatedBy;
                this.UpdatedDate = dynamicForm.UpdatedDate;
                this.ApplicationPageDTO = dynamicForm.ApplicationPageID.HasValue ? new ApplicationPageDTO(dynamicForm.sysBpmsApplicationPage) : null;
                this.IsEncrypted = this.ConfigXML.ToStringObj().ParseXML<DynamicFormConfigXmlModel>()?.IsEncrypted ?? false;
            }
        }
        [DataMember]
        public Guid ID { get; set; }
        [DataMember]
        public Guid? ProcessId { get; set; }
        [DataMember]
        public Guid? ApplicationPageID { get; set; }
        [DataMember]
        public string DesignJson { get; set; }
        [DataMember]
        public string ConfigXML { get; set; } 
        [Required]
        [DataMember]
        public string Name { get; set; } 
        [DataMember]
        public int Version { get; set; } 
        [DataMember]
        public bool? ShowInOverview { get; set; }
        [DataMember]
        public string OnExitFormCode { get; set; }
        [DataMember]
        public string OnEntryFormCode { get; set; }
        [DataMember]
        public DesignCodeModel OnExitFormCodeDesign
        {
            get
            {
                return DesignCodeUtility.GetDesignCodeFromXml(this.OnExitFormCode);
            }
            private set { }
        }
        public DesignCodeModel OnEntryFormCodeDesign
        {
            get
            {
                return DesignCodeUtility.GetDesignCodeFromXml(this.OnEntryFormCode);
            }
            private set { }
        } 
        [DataMember]
        public bool IsEncrypted { get; set; } 
        [DataMember]
        public string CreatedBy { get; set; } 
        [DataMember]
        public string CreatedByName { get { return DotNetNuke.Entities.Users.UserController.GetUserByName(this.CreatedBy)?.DisplayName ?? this.CreatedBy; } }
 
        [DataMember]
        public System.DateTime CreatedDate { get; set; } 
        [DataMember]
        public string UpdatedBy { get; set; } 
        [DataMember]
        public string UpdatedByName { get { return DotNetNuke.Entities.Users.UserController.GetUserByName(this.UpdatedBy)?.DisplayName ?? this.UpdatedBy; } }
 
        [DataMember]
        public System.DateTime UpdatedDate { get; set; }
        [DataMember]
        public ApplicationPageDTO ApplicationPageDTO { get; set; }
    }
}