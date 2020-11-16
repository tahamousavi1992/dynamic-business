﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    /// <summary>
    /// for using in EntityDef Properties
    /// </summary>
    [DataContract]
    public class EntityPropertyModel
    {
        public EntityPropertyModel() { }

        public EntityPropertyModel(string ID, string Name, bool Required, e_dbType dbType, string defaultValue, string length)
        {
            this.Load(ID, Name, Required, dbType, defaultValue, length);
            this.IsActive = true;
        }

        public void Load(string ID, string Name, bool Required, e_dbType dbType, string defaultValue, string length)
        {
            this.ID = ID;
            this.Name = Name;
            this.Required = Required;
            this.DbType = dbType;
            this.DefaultValue = defaultValue;
            this.Length = length;
        }
        [DataMember]
        public string ID { get; set; } 
        [DataMember]
        public string Name { get; set; } 
        [DataMember]
        public bool Required { get; set; } 
        [DataMember]
        public e_dbType DbType { get; set; } 
        [DataMember]
        public string DbTypeName { get { return this.DbType.ToString(); } set { } } 
        [DataMember]
        public string DefaultValue { get; set; } 
        [DataMember]
        public string Length { get; set; } 
        [DataMember]
        public bool IsActive { get; set; } 
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public string SqlTypeName
        {
            get
            {
                switch (this.DbType)
                {
                    case e_dbType.Uniqueidentifier:
                        return "[uniqueidentifier]";
                    case e_dbType.DateTime:
                        return "[datetime]";
                    case e_dbType.Decimal:
                        return "[Decimal]" + (string.IsNullOrWhiteSpace(this.Length) ? "" : ($"({ this.Length.Trim(')').Trim('(')})"));
                    case e_dbType.Integer:
                        return "[int]";
                    case e_dbType.Long:
                        return "[bigint]";
                    case e_dbType.boolean:
                        return "[bit]";
                    case e_dbType.String:
                        return $"[nvarchar]({(string.IsNullOrWhiteSpace(this.Length) ? "max" : this.Length)})";
                };
                return "";
            }
            set { }
        }
        [DataMember]
        public string CSharpTypeName
        {
            get
            {
                switch (this.DbType)
                {
                    case e_dbType.Uniqueidentifier:
                        return "Guid" + (this.Required ? "" : "?");
                    case e_dbType.DateTime:
                        return "DateTime" + (this.Required ? "" : "?");
                    case e_dbType.Decimal:
                        return "decimal" + (this.Required ? "" : "?");
                    case e_dbType.Integer:
                        return "int" + (this.Required ? "" : "?");
                    case e_dbType.Long:
                        return "long" + (this.Required ? "" : "?");
                    case e_dbType.boolean:
                        return "bool" + (this.Required ? "" : "?");
                    case e_dbType.String:
                        return "string";
                };
                return "";
            }
            set { }
        }
        [DataMember]
        public string SqlDefaultValue
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(this.DefaultValue))
                {
                    switch (this.DbType)
                    {
                        case e_dbType.DateTime:
                            return $"DEFAULT '{this.DefaultValue}'";
                        case e_dbType.Decimal:
                            return $"DEFAULT '{this.DefaultValue}'";
                        case e_dbType.Integer:
                            return $"DEFAULT '{this.DefaultValue}'";
                        case e_dbType.Long:
                            return $"DEFAULT '{this.DefaultValue}'";
                        case e_dbType.String:
                            return $"DEFAULT '{this.DefaultValue}'";
                        case e_dbType.Uniqueidentifier:
                            return $"DEFAULT '{this.DefaultValue}'";
                        case e_dbType.boolean:
                            return $"DEFAULT '{this.DefaultValue}'";
                    };
                }
                return "";
            }
            set { }
        }

        public enum e_dbType
        {
            [Description("String")]
            String = 0,
            [Description("Integer")]
            Integer = 1,
            [Description("Decimal")]
            Decimal = 3,
            [Description("Long")]
            Long = 4,
            [Description("DateTime")]
            DateTime = 5,
            [Description("Uniqueidentifier")]
            Uniqueidentifier = 6,
            [Description("boolean")]
            boolean = 7,
        }

    }
}
