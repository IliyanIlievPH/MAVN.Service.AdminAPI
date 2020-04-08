using FluentValidation;
using MAVN.Service.AdminAPI.Models.ActionRules;
using MAVN.Service.AdminAPI.Models.BurnRules;
using MAVN.Service.AdminAPI.Validators.ActionRules;

namespace MAVN.Service.AdminAPI.Validators.BurnRules
{
    public class BurnRuleCreateRequestValidator : BurnRuleBaseRequestValidator<BurnRuleCreateRequest, MobileContentCreateRequest>
    {
        public BurnRuleCreateRequestValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            When(o => o.MobileContents != null, () =>
            {
                RuleForEach(o => o.MobileContents)
                    .SetValidator(new MobileContentCreateRequestValidator<MobileContentCreateRequest>());
            });
        }
    }
}
