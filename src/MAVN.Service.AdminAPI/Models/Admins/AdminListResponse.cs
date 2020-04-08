using System.Collections.Generic;
using JetBrains.Annotations;
using MAVN.Service.AdminAPI.Models.Common;

namespace MAVN.Service.AdminAPI.Models.Admins
{
    /// <summary>
    /// Represents a paginated collection of admins.
    /// </summary>
    [PublicAPI]
    public class AdminListResponse
    {
        /// <summary>
        /// The page details.
        /// </summary>
        public PagedResponseModel PagedResponse { get; set; }

        /// <summary>
        /// A collection of admins.
        /// </summary>
        public IReadOnlyList<AdminModel> Items { get; set; }
    }
}
