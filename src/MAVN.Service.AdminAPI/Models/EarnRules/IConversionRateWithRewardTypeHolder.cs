using MAVN.Service.AdminAPI.Models.ActionRules;

namespace MAVN.Service.AdminAPI.Models.EarnRules
{
    public interface IConversionRateWithRewardTypeHolder : IConversionRateHolder
    {
        RewardType RewardType { get; set; }
    }
}
