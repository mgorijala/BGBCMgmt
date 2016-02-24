using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGBC.Model.Metadata
{
    public class ProductOrderMetadata
    {
        [DataType(DataType.Currency)]
        public Nullable<decimal> Price { get; set; }
    }
}
