using System.Collections.Generic;
using JetBrains.Annotations;
using MAVN.Service.AdminAPI.Models.Common;

namespace MAVN.Service.AdminAPI.Models.Payments
{
    /// <summary>
    /// Represents a paginated list of payments.
    /// </summary>
    [PublicAPI]
    public class PaymentListModel
    {
        /// <summary>
        /// The paging model
        /// </summary>
        public PagedResponseModel PagedResponse { get; set; }

        /// <summary>
        /// The burn rules
        /// </summary>
        public IReadOnlyList<PaymentModel> Items { get; set; }
    }
}
