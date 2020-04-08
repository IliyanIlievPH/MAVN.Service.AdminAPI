using FluentValidation;
using JetBrains.Annotations;
using MAVN.Service.AdminAPI.Models.EarnRules;

namespace MAVN.Service.AdminAPI.Validators.EarnRules
{
    [UsedImplicitly]
    public class ConditionCreateModelValidator : ConditionBaseModelValidator<ConditionCreateModel>
    {
        public ConditionCreateModelValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;
        }
    }
}
