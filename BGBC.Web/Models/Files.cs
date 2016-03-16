using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BGBC.Web.Models
{
    public class Files
    {   
        public int ID { get; set; }
        
        public int TenantID { get; set; } 
        public string ContentType { get; set; }
        public byte[] FileContent { get; set; } 
        public string FileName { get; set; }
    }
}