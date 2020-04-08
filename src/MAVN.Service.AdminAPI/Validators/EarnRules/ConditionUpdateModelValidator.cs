using Common;
using FluentValidation;
using JetBrains.Annotations;
using MAVN.Service.AdminAPI.Models.EarnRules;

namespace MAVN.Service.AdminAPI.Validators.EarnRules
{
    [UsedImplicitly]
    public class ConditionUpdateModelValidator : ConditionBaseModelValidator<ConditionUpdateModel>
    {
        public ConditionUpdateModelValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(x => x.Id)
                .Must(x => string.IsNullOrEmpty(x) || x.IsGuid())
                .WithMessage("Id should be guid");
        }
    }
}
