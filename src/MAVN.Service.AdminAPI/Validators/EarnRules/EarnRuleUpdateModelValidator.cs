using System.Linq;
using Common;
using FluentValidation;
using MAVN.Service.AdminAPI.Models.ActionRules;
using MAVN.Service.AdminAPI.Models.EarnRules;
using MAVN.Service.AdminAPI.Validators.ActionRules;

namespace MAVN.Service.AdminAPI.Validators.EarnRules
{
    public class EarnRuleUpdateModelValidator : EarnRuleBaseModelValidator<EarnRuleUpdateModel>
    {
        public EarnRuleUpdateModelValidator()
        {
            RuleFor(o => o.Id)
                .NotEmpty()
                .WithMessage("Id required")
                .Must(o => o.IsGuid())
                .WithMessage("Id should be guid");

            RuleFor(o => o.Conditions)
                .NotEmpty()
                .WithMessage("Conditions required")
                .Must(o => o.GroupBy(x => x.Type).All(x => x.Count() == 1))
                .WithMessage("Conditions should be unique");

            RuleForEach(o => o.Conditions)
                .SetValidator(new ConditionUpdateModelValidator());

            RuleForEach(o => o.MobileContents)
                .SetValidator(new MobileContentEditRequestValidator());

            RuleFor(o => o.MobileContents)
                .Must(contents => contents != null && contents.Any())
                .WithMessage(o => $"There should be at least one item in the {nameof(o.MobileContents)} value")
                .Must(contents =>
                {
                    return contents.Any(c => c.MobileLanguage == MobileLocalization.En);
                })
                .WithMessage("English content is required.");
        }
    }
}
