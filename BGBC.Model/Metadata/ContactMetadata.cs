using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGBC.Model.Metadata
{
    public class ContactMetadata
    {
        [DataType(DataType.EmailAddress)]
        public string EMail { get; set; }
    }
}
