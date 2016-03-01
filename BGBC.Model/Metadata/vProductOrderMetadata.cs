using System;
using System.ComponentModel.DataAnnotations;

namespace BGBC.Model.Metadata
{
    class vProductOrderMetadata
    {
        [DataType(DataType.Currency)]
        public Nullable<decimal> Price { get; set; }
    }
}
