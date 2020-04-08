using JetBrains.Annotations;
using Lykke.Service.Campaign.Client.Models.Condition;

namespace MAVN.Service.AdminAPI.Models.EarnRules
{
    /// <summary>
    /// Represents condition edit details.
    /// </summary>
    [PublicAPI]
    public class ConditionUpdateModel : ConditionBaseModel
    {
        /// <summary>
        /// The identifier of the condition.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Represents a condition reward ratio attribute
        /// </summary>
        public RewardRatioAttribute RewardRatio { get; set; }
    }
}
