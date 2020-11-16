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
    public partial class DBConnectionDTO
    {
        [DataMember]
        public Guid ID { get; set; }

        [Required] 
        [DataMember]
        public string Name { get; set; }

        [Required] 
        [DataMember]
        public string DataSource { get; set; }

        [Required] 
        [DataMember]
        public string InitialCatalog { get; set; }
         
        [DataMember]
        public string UserID { get; set; }
         
        [DataMember]
        public string Password { get; set; }
         
        [UIHint("_YesOrNoTemplate")]
        [DataMember]
        public bool IntegratedSecurity { get; set; }

        public DBConnectionDTO() { }
        public DBConnectionDTO(sysBpmsDBConnection dBConnection)
        {
            this.ID = dBConnection.ID;
            this.Name = dBConnection.Name;
            this.DataSource = dBConnection.DataSource;
            this.InitialCatalog = dBConnection.InitialCatalog;
            this.UserID = dBConnection.UserID;
            this.Password = dBConnection.Password;
            this.IntegratedSecurity = dBConnection.IntegratedSecurity;
        }
    }
}