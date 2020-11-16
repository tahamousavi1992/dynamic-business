using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using DynamicBusiness.BPMS.Domain;
namespace DynamicBusiness.BPMS.SharedPresentation
{
    [DataContract]
    public class SelectTypeControlDTO
    {
        public SelectTypeControlDTO() { }
        public SelectTypeControlDTO(string containerID, string value, DCBaseModel.e_ValueType valueType,
            string parentShapeID, string shapeID, bool? isOutputYes, string dynamicFormID)
        {
            this.ContainerID = containerID;
            this.Value = value;
            this.ValueType = valueType;
            this.ParentShapeID = parentShapeID;
            this.ShapeID = shapeID;
            this.IsOutputYes = isOutputYes;
            this.DynamicFormID = dynamicFormID;
        }
        [DataMember]
        public string ContainerID { get; set; }
        [DataMember]
        public string Value { get; set; }
        [DataMember]
        public DCBaseModel.e_ValueType ValueType { get; set; }
        [DataMember]
        public string ParentShapeID { get; set; }
        [DataMember]
        public string ShapeID { get; set; }
        [DataMember]
        public bool? IsOutputYes { get; set; }
        [DataMember]
        public string DynamicFormID { get; set; }
    }
}