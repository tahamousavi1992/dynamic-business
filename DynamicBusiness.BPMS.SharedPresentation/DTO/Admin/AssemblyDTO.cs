using DynamicBusiness.BPMS.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace DynamicBusiness.BPMS.SharedPresentation
{
    [DataContract]
    public class AssemblyDTO
    {
        public AssemblyDTO() { }
        public AssemblyDTO(FileInfo fileInfo)
        {
            this.FileName = fileInfo.Name;
            this.Name = Path.GetFileNameWithoutExtension(fileInfo.Name);
            this.Size = (fileInfo.Length / 1024);
            this.Version = FileVersionInfo.GetVersionInfo(fileInfo.FullName).ProductVersion;
        } 
        [Required]
        [DataMember]
        public string FileName { get; set; }
         
        [Required]
        [DataMember]
        public string StyleCode { get; set; }
         
        [DataMember]
        public long Size { get; set; }
         
        [DataMember]
        public string Name { get; set; }
         
        [DataMember]
        public string Version { get; set; }
    }
}