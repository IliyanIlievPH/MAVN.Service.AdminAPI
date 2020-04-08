using Common;
using FluentValidation;
using JetBrains.Annotations;
using MAVN.Service.AdminAPI.Models.ActionRules;
using MAVN.Service.AdminAPI.Models.BurnRules;
using MAVN.Service.AdminAPI.Validators.ActionRules;

namespace MAVN.Service.AdminAPI.Validators.BurnRules
{
    [UsedImplicitly]
    public class BurnRuleUpdateModelValidator : BurnRuleBaseRequestValidator<BurnRuleUpdateRequest, MobileContentEditRequest>
    {
        public BurnRuleUpdateModelValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(o => o.Id)
                .NotEmpty()
                .WithMessage("Id required")
                .Must(o => o.IsGuid())
                .WithMessage("Id should be guid");

            When(o => o.MobileContents != null, () =>
            {
                RuleForEach(o => o.MobileContents)
                    .SetValidator(new MobileContentEditRequestValidator());
            });
        }
    }
}
