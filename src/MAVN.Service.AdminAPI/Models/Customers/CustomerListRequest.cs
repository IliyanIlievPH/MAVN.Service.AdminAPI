using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace MAVN.Service.AdminAPI.Models.Customers
{
    /// <summary>
    /// Represents a paginated list request parameters.
    /// </summary>
    [PublicAPI]
    public class CustomerListRequest
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

        /// <summary>
        /// The customer email or identifier to filter collection.
        /// </summary>
        public string SearchValue { get; set; }
    }
}
