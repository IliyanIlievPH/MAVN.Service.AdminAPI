using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace MAVN.Service.AdminAPI.Models.Admins
{
    /// <summary>
    /// Represents a paginated list request parameters.
    /// </summary>
    [PublicAPI]
    public class AdminListRequest
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
        /// The admin email to filter collection.
        /// </summary>
        public string SearchValue { get; set; }

        /// <summary>
        /// Whether to select only Active or Non-active users. Null (default value) for both.
        /// </summary>
        public bool? Active { get; set; }
    }
}
