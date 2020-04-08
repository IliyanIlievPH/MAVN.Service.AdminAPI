using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace MAVN.Service.AdminAPI.Models.Payments
{
    /// <summary>
    /// Represents a paginated list request parameters.
    /// </summary>
    [PublicAPI]
    public class PaymentListRequest
    {
        /// <summary>
        /// The number of items per page.
        /// </summary>
        [Range(1, 1000)]
        public int PageSize { get; set; }

        /// <summary>
        /// The current page index.
        /// </summary>
        [Range(1, int.MaxValue)]
        public int CurrentPage { get; set; }
    }
}
