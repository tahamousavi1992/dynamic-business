using DynamicBusiness.BPMS.BusinessLogic;
using DynamicBusiness.BPMS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace DynamicBusiness.BPMS.SharedPresentation
{
    [DataContract]
    public class DesignCodeDTO
    {
        public enum e_CodeType
        {
            TaskAccessRule = 1,
            TaskServiceCode = 2,
            ConditionCode = 3,
            DynamicForm = 4,
            ButtonSave = 5
        }

        [DataMember]
        public string Code { get; set; }
        [DataMember]
        public string DesignCode { get; set; }
        [DataMember]
        public e_CodeType CodeType { get; set; }
        [DataMember]
        public string CallBack { get; set; }
        /// <summary>
        ///after rendering all expression the assemblies are sum with each other and reserved into this property
        /// </summary>
        [DataMember]
        public string Assemblies { get; set; }
        [DataMember]
        public string ID { get; set; }
        [DataMember]
        public List<object> CodeObjects { get; set; }
        [DataMember]
        public Guid? DynamicFormID { get; set; }
        [DataMember]
        public string Diagram { get; set; }

        [DataMember]
        public object DesignCodeData { get; set; }
        [DataMember]
        public List<ComboTreeModel> ProcessVariables { get; set; }
        [DataMember]
        public List<ComboTreeModel> AssembliesJson { get; set; }
        [DataMember]
        public List<QueryModel> ApplicationPages { get; set; }
        [DataMember]
        public List<LURowDTO> DepartmentRoles { get; set; }
        [DataMember]
        public List<QueryModel> DepartmentList { get; set; }
        [DataMember]
        public List<DocumentFolderDTO> DocumentFolders { get; set; }
        [DataMember]
        public List<QueryModel> ProcessControls { get; set; }
    }
}