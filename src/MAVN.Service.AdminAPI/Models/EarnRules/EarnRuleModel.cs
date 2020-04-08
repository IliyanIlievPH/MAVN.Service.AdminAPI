using System.Collections.Generic;
using JetBrains.Annotations;
using MAVN.Service.AdminAPI.Models.ActionRules;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MAVN.Service.AdminAPI.Models.EarnRules
{
    /// <summary>
    /// Represent an earn rule.
    /// </summary>
    [PublicAPI]
    public class EarnRuleModel : EarnRuleBaseModel
    {
        public EarnRuleModel()
        {
            Conditions = new ConditionModel[0];
        }

        /// <summary>
        /// The identifier if the earn rule.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The name of reward asset.
        /// </summary>
        public string Asset { get; set; }
        
        /// <summary>
        /// The status of the earn rule.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public EarnRuleStatus Status { get; set; }

        /// <summary>
        /// A collection of earn rule conditions.
        /// </summary>
        public IReadOnlyList<ConditionModel> Conditions { get; set; }

        /// <summary>
        /// A collection of earn rule contents.
        /// </summary>
        public IReadOnlyList<MobileContentResponse> MobileContents { get; set; }
    }
}
