using DynamicBusiness.BPMS.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace DynamicBusiness.BPMS.SharedPresentation
{
    [DataContract]
    public class JsFileDTO
    {
        public JsFileDTO(FileInfo fileInfo)
        {
            this.FileName = fileInfo.Name;
            this.Name = Path.GetFileNameWithoutExtension(fileInfo.Name);
            this.Size = (fileInfo.Length / 1024);
        } 
        [DataMember]
        public string FileName { get; set; } 
        [DataMember]
        public long Size { get; set; } 
        [DataMember]
        public string Name { get; set; }
    }
}