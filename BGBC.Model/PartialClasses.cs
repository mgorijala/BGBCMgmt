using BGBC.Model.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGBC.Model
{
    [MetadataType(typeof(ProductMetadata))]
    public partial class Product
    {

    }

    [MetadataType(typeof(PropertyMetadata))]
    public partial class Property
    {

    }
    [MetadataType(typeof(TenantMetadata))]
    public partial class Tenant
    {

    }

    [MetadataType(typeof(UserMetadata))]
    public partial class User
    {

    }

    [MetadataType(typeof(ContactMetadata))]
    public partial class ContactForm
    {

    }

    [MetadataType(typeof(ProfileMetadata))]
    public partial class Profile
    {

    }

    [MetadataType(typeof(TenantRefMetadata))]
    public partial class TenantReferral
    {

    }
    [MetadataType(typeof(UserReferenceMetadata))]
    public partial class UserReference
    {

    }

    [MetadataType(typeof(vRentPaymentMetadata))]
    public partial class vRentPayment
    {

    }

    [MetadataType(typeof(ProductOrderMetadata))]
    public partial class ProductOrder
    {

    }
}
