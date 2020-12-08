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
    public class EntityDefDTO
    {
        public EntityDefDTO() { }
        public EntityDefDTO(sysBpmsEntityDef entityDef)
        {
            if (entityDef != null)
            {
                this.ID = entityDef.ID;
                this.DesignXML = entityDef.DesignXML;
                this.IsActive = entityDef.IsActive;
                this.Name = entityDef.Name; 
                this.DisplayName = entityDef.DisplayName;
            }
        }
        [DataMember]
        public Guid ID { get; set; }
        [DataMember]
        public string DesignXML { get; set; } 
        [DataMember]
        public bool IsActive { get; set; }

        [Required] 
        [DataMember]
        public string DisplayName { get; set; }

        [Required] 
        [DataMember]
        public string Name { get; set; }
 
        [DataMember]
        public string FormattedTableName
        {
            get { return "Bpms_" + this.Name; } private set { }
        }
        [DataMember]
        public List<EntityPropertyModel> _Properties { get; set; }
        [DataMember]
        public List<EntityPropertyModel> Properties
        {
            get
            {
                if (this._Properties == null)
                    this._Properties = this.DesignXML.ParseXML<EntityDesignXmlModel>().EntityPropertyModel ?? new List<EntityPropertyModel>();
                return _Properties;
            }
            set
            {
                _Properties = value;
            }
        }

        /// <summary>
        ///retrieve custom properties with systemic one like ID
        /// </summary>
        [DataMember]
        public List<EntityPropertyModel> AllProperties
        {
            get
            {

                return Properties.Union(new List<EntityPropertyModel>() { new EntityPropertyModel() { Name = "ID", ID = "1" } }).ToList();
            }private set { }
        }

    }
}