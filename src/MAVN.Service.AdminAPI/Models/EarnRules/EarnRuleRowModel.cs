using JetBrains.Annotations;
using MAVN.Service.AdminAPI.Models.Partners;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MAVN.Service.AdminAPI.Models.EarnRules
{
    /// <summary>
    /// Represent a earn rule row.
    /// </summary>
    [PublicAPI]
    public class EarnRuleRowModel : EarnRuleBaseModel
    {
        /// <summary>
        /// The identifier of the earn rule.
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
        /// Earn rule's business vertical
        /// </summary>
        public BusinessVertical? Vertical { get; set; }
    }
}
