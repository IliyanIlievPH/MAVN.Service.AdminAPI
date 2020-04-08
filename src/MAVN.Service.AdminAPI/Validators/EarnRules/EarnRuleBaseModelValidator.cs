using System;
using FluentValidation;
using JetBrains.Annotations;
using MAVN.Service.AdminAPI.Models.EarnRules;
using MAVN.Service.AdminAPI.Validators.ActionRules;

namespace MAVN.Service.AdminAPI.Validators.EarnRules
{
    [UsedImplicitly]
    public abstract class EarnRuleBaseModelValidator<T> : AbstractValidator<T>
        where T : EarnRuleBaseModel
    {
        protected EarnRuleBaseModelValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(o => o.Name)
                .NotEmpty()
                .WithMessage("Name required")
                .Length(3, 50)
                .WithMessage("Name should be between 3 and 50 chars");

            RuleFor(o => o.Reward)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Reward should be greater than or equal to zero")
                .Must((o, p) => o != null && (o.RewardType != RewardType.Fixed ||
                                              o.RewardType == RewardType.Fixed && o.Reward <= int.MaxValue))
                .WithMessage($"Fixed reward should be less than or equal to {int.MaxValue:N}")
                .Must((model, value) => model.RewardType != RewardType.Percentage || value >= 0 && value <= 100)
                .WithMessage("Reward percentage should be between 0 and 100");

            RuleFor(m => m.ApproximateAward)
                .NotNull()
                .GreaterThanOrEqualTo(0m)
                .WithMessage("Approximate Award is required when Percentage or Conversion Reward Type is selected")
                .When(model => model.RewardType == RewardType.Percentage || model.RewardType == RewardType.ConversionRate);

            // Validator ScalePrecision works only with decimal
            RuleFor(c => c.RewardDecimal)
                .ScalePrecision(2, 5, false)
                .When(c => c.RewardType == RewardType.Percentage);

            Include(new ConversionRateWithRewardTypeValidator());

            RuleFor(o => o.RewardType)
                .NotEqual(RewardType.None)
                .WithMessage("Reward type required");

            RuleFor(o => o.FromDate)
                .GreaterThan(DateTime.MinValue)
                .WithMessage("Date from required");

            RuleFor(o => o.ToDate)
                .Must((model, value) => value == null || model.FromDate.Date <= value.Value.Date)
                .WithMessage("End date should be greater than start date");

            RuleFor(o => o.CompletionCount)
                .GreaterThan(0)
                .WithMessage("Completion count should be greater than zero");

            RuleFor(o => o.Description)
                .NotEmpty()
                .WithMessage("Description required")
                .Length(3, 1000)
                .WithMessage("Description should be between 3 and 1000 chars");
        }
    }
}
