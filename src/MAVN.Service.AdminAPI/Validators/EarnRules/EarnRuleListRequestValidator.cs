using FluentValidation;
using JetBrains.Annotations;
using MAVN.Service.AdminAPI.Models.EarnRules;

namespace MAVN.Service.AdminAPI.Validators.EarnRules
{
    [UsedImplicitly]
    public class EarnRuleListRequestValidator : AbstractValidator<EarnRuleListRequest>
    {
        public EarnRuleListRequestValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(o => o.EarnRuleName)
                .Must(x => string.IsNullOrEmpty(x) || x.Length <= 100)
                .WithMessage("EarnRuleName length should be less or equal 100 chars");
        }
    }
}
