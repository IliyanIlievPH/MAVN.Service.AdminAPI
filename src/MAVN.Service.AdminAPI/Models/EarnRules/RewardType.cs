using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MAVN.Service.AdminAPI.Models.EarnRules
{
    /// <summary>
    /// Specifies a campaign reward types.
    /// </summary>
    [PublicAPI]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum RewardType
    {
        /// <summary>
        /// Unspecified type.
        /// </summary>
        None,

        /// <summary>
        /// Fixed amount in the base currency.
        /// </summary>
        Fixed,

        /// <summary>
        /// Percentage of given amount.
        /// </summary>
        Percentage,
        
        /// <summary>
        /// Conversion rate of given amount.
        /// </summary>
        ConversionRate
    }
}
