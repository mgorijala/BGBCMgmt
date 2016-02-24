using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGBC.Model.Metadata
{
    class vRentPaymentMetadata
    {

        [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy}")]
        public System.DateTime TransDate { get; set; }
    }
}
