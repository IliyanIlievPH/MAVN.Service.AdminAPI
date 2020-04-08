using System.Collections.Generic;
using MAVN.Service.AdminAPI.Models.Common;

namespace MAVN.Service.AdminAPI.Models.BurnRules
{
    /// <summary>
    /// Represents burn rules response from a list request
    /// </summary>
    public class BurnRulesListResponse
    {
        /// <summary>
        /// The paging model
        /// </summary>
        public PagedResponseModel PagedResponse { get; set; }

        /// <summary>
        /// The burn rules
        /// </summary>
        public IEnumerable<BurnRuleInfoModel> BurnRules { get; set; }
    }
}
