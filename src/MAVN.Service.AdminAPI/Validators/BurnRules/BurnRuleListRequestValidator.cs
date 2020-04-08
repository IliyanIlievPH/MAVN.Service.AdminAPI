using FluentValidation;
using MAVN.Service.AdminAPI.Models.BurnRules;

namespace MAVN.Service.AdminAPI.Validators.BurnRules
{
    public class BurnRuleListRequestValidator : AbstractValidator<BurnRuleListRequest>
    {
        public BurnRuleListRequestValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(o => o.Title)
                .Must(x => string.IsNullOrEmpty(x) || x.Length <= 100)
                .WithMessage("Title length should be less or equal 100 chars");
        }   
    }
}
