using System.Collections.Generic;
using MAVN.Service.AdminAPI.Models.ActionRules;

namespace MAVN.Service.AdminAPI.Models.EarnRules
{
    /// <summary>
    /// Represents earn rule created response
    /// </summary>
    public class EarnRuleCreatedResponse
    {
        /// <summary>
        /// Represents the id of the earn rule
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Represents the image contents created for the earn rule
        /// </summary>
        public IEnumerable<ImageContentCreatedResponse> CreatedImageContents { get; set; }
    }
}
