using System.Collections.Generic;
using JetBrains.Annotations;
using MAVN.Service.AdminAPI.Models.Common;

namespace MAVN.Service.AdminAPI.Models.Customers
{
    [PublicAPI]
    public class CustomerListResponse
    {
        public PagedResponseModel PagedResponse { get; set; }

        public IEnumerable<CustomerModel> Customers { get; set; }
    }
}
