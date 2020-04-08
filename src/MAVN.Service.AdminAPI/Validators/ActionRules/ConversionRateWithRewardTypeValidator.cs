using FluentValidation;
using MAVN.Service.AdminAPI.Models.EarnRules;

namespace MAVN.Service.AdminAPI.Validators.ActionRules
{
    public class ConversionRateWithRewardTypeValidator : AbstractValidator<IConversionRateWithRewardTypeHolder>
    {
        public ConversionRateWithRewardTypeValidator()
        {
            When(x => x.RewardType == RewardType.ConversionRate, () =>
            {
                Include(new ConversionRateValidator());
            });
        }
    }
}
