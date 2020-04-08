using System.Collections.Generic;
using JetBrains.Annotations;
using MAVN.Service.AdminAPI.Models.Common;

namespace MAVN.Service.AdminAPI.Models.EarnRules
{
    /// <summary>
    /// Represents earn rule List Response
    /// </summary>
    [PublicAPI]
    public class EarnRuleListResponse
    {
        /// <summary>
        /// Paging model
        /// </summary>
        public PagedResponseModel PagedResponse { get; set; }

        /// <summary>
        /// List of earn rules
        /// </summary>
        public IEnumerable<EarnRuleRowModel> EarnRules { get; set; }
    }
}
