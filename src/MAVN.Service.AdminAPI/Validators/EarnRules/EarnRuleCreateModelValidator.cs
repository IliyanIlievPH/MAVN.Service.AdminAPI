using System.Linq;
using FluentValidation;
using JetBrains.Annotations;
using Lykke.Service.Campaign.Client.Models.Enums;
using MAVN.Service.AdminAPI.Models.ActionRules;
using MAVN.Service.AdminAPI.Models.EarnRules;
using MAVN.Service.AdminAPI.Validators.ActionRules;

namespace MAVN.Service.AdminAPI.Validators.EarnRules
{
    [UsedImplicitly]
    public class EarnRuleCreateModelValidator : EarnRuleBaseModelValidator<EarnRuleCreateModel>
    {
        public EarnRuleCreateModelValidator()
        {
            RuleFor(o => o.Conditions)
                .NotEmpty()
                .WithMessage("Conditions required")
                .Must(o => o.GroupBy(x => x.Type).All(x => x.Count() == 1))
                .WithMessage("Conditions should be unique");

            RuleForEach(o => o.Conditions)
                .SetValidator(new ConditionCreateModelValidator());

            RuleForEach(o => o.MobileContents)
                .SetValidator(new MobileContentCreateRequestValidator<MobileContentCreateRequest>());

            RuleFor(o => o.MobileContents)
                .Must(contents => contents != null && contents.Any())
                .WithMessage(o => $"There should be at least one item in the {nameof(o.MobileContents)} value")
                .Must(contents =>
                {
                    return contents.Any(c => c.MobileLanguage == Localization.En);
                })
                .WithMessage("English content is required.");
        }
    }
}
