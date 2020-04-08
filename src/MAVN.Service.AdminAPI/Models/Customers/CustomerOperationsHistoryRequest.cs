using JetBrains.Annotations;
using MAVN.Service.AdminAPI.Models.Common;

namespace MAVN.Service.AdminAPI.Models.Customers
{
    [PublicAPI]
    public class CustomerOperationsHistoryRequest : PagedRequestModel
    {
        /// <summary>
        /// The customer id.
        /// </summary>
        public string CustomerId { get; set; }
    }
}
