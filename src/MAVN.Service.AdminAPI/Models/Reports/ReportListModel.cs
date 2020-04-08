using System.Collections.Generic;
using JetBrains.Annotations;
using MAVN.Service.AdminAPI.Models.Common;

namespace MAVN.Service.AdminAPI.Models.Reports
{
    /// <summary>
    /// Represents a paginated list of transactions.
    /// </summary>
    [PublicAPI]
    public class ReportListModel
    {
        /// <summary>
        /// The paging model
        /// </summary>
        public PagedResponseModel PagedResponse { get; set; }

        /// <summary>
        /// Report items
        /// </summary>
        public IReadOnlyList<ReportItemModel> Items { get; set; }
    }
}
