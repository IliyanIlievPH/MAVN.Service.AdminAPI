using FluentValidation;
using JetBrains.Annotations;
using MAVN.Service.AdminAPI.Models.Settings;

namespace MAVN.Service.AdminAPI.Validators.Settings
{
    [UsedImplicitly]

    public class AgentRequirementUpdateRequestValidator : AbstractValidator<AgentRequirementUpdateRequest>
    {
        public AgentRequirementUpdateRequestValidator()
        {
            RuleFor(m => m.TokensAmount)
                .NotNull()
                .NotEmpty()
                .WithMessage("Reward should not be empty")
                .GreaterThanOrEqualTo(0m)
                .WithMessage("The amount of tokens should be equal or greater than 0.");
        }
    }
}
