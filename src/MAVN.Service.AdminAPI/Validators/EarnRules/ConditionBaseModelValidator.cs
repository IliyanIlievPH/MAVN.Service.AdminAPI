using FluentValidation;
using JetBrains.Annotations;
using MAVN.Service.AdminAPI.Models.EarnRules;
using MAVN.Service.AdminAPI.Validators.ActionRules;

namespace MAVN.Service.AdminAPI.Validators.EarnRules
{
    [UsedImplicitly]
    public class ConditionBaseModelValidator<T> : AbstractValidator<T>
        where T : ConditionBaseModel
    {
        public ConditionBaseModelValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(o => o.Type)
                .NotEmpty()
                .WithMessage("Type required");

            RuleFor(o => o.ImmediateReward)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Immediate reward should be greater than or equal to zero")
                .Must((model, value) => model.RewardType != RewardType.Percentage || (value >= 0 && value <= 100))
                .WithMessage("Reward in percentage should be between 0 and 100");

            // Validator ScalePrecision works only with decimal
            RuleFor(c => c.ImmediateReward)
                .ScalePrecision(2, 5, false)
                .When(c => c.RewardType == RewardType.Percentage);

            RuleFor(m => m.ApproximateAward)
                .NotNull()
                .GreaterThanOrEqualTo(0m)
                .WithMessage("Approximate Award is required when Percentage or Conversion Reward Type is selected")
                .When(model => model.RewardType == RewardType.Percentage || model.RewardType == RewardType.ConversionRate);

            Include(new ConversionRateWithRewardTypeValidator());

            #region Staking

            RuleFor(c => c.StakeAmount)
                .NotNull()
                .GreaterThan(0)
                .When(c => c.HasStaking);

            RuleFor(c => c.StakeWarningPeriod)
                .NotNull()
                .GreaterThanOrEqualTo(0)
                .When(c => c.HasStaking);

            RuleFor(c => c.StakingPeriod)
                .NotNull()
                .GreaterThan(0)
                .When(c => c.HasStaking);

            RuleFor(c => c.StakingRule)
                .NotNull()
                .GreaterThanOrEqualTo(0)
                .LessThanOrEqualTo(100)
                .ScalePrecision(2, 5, false)
                .When(c => c.HasStaking);

            RuleFor(c => c.BurningRule)
                .NotNull()
                .GreaterThanOrEqualTo(0)
                .LessThanOrEqualTo(100)
                .ScalePrecision(2, 5, false)
                .When(c => c.HasStaking);

            #endregion
        }
    }
}
