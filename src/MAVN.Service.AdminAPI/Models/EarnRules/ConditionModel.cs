using JetBrains.Annotations;
using MAVN.Service.Campaign.Client.Models.Condition;

namespace MAVN.Service.AdminAPI.Models.EarnRules
{
    /// <summary>
    /// Represents a earn rule condition.
    /// </summary>
    [PublicAPI]
    public class ConditionModel : ConditionBaseModel
    {
        /// <summary>
        /// The identifier of the condition.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The display name of the condition.
        /// </summary>
        public string DisplayName { get; set; }

        public RewardRatioAttributeDetailsResponseModel RewardRatio { get; set; }
    }
}
