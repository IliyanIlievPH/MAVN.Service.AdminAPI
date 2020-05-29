using JetBrains.Annotations;
using MAVN.Service.Campaign.Client.Models.Condition;

namespace MAVN.Service.AdminAPI.Models.EarnRules
{
    /// <summary>
    /// Represents a condition creation details.
    /// </summary>
    [PublicAPI]
    public class ConditionCreateModel : ConditionBaseModel
    {
        public RewardRatioAttribute RewardRatio { get; set; }
    }
}
