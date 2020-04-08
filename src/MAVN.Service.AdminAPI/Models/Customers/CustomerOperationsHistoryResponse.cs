using System.Collections.Generic;
using MAVN.Service.AdminAPI.Models.Common;

namespace MAVN.Service.AdminAPI.Models.Customers
{
    /// <summary>
    /// Represents operations history
    /// </summary>
    public class CustomerOperationsHistoryResponse
    {
        public IReadOnlyCollection<CustomerOperationModel> Operations { get; set; }
        public PagedResponseModel PagedResponse { get; set; }

        public CustomerOperationsHistoryResponse()
        {
            Operations = new List<CustomerOperationModel>();
        }
    }
}
