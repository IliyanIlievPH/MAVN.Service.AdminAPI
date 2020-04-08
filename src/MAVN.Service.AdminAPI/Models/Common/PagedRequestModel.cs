using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace MAVN.Service.AdminAPI.Models.Common
{
    /// <summary>
    /// Model for getting paged results
    /// </summary>
    [PublicAPI]
    public class PagedRequestModel
    {
        /// <summary>
        /// Represents page size
        /// </summary>
        [Range(1, 500)]
        public int PageSize { get; set; }

        /// <summary>
        /// Represents current page number
        /// </summary>
        [Range(1, 10_000)]
        public int CurrentPage { get; set; }
    }
}
