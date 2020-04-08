using FluentValidation;
using JetBrains.Annotations;
using MAVN.Service.AdminAPI.Models.Settings;

namespace MAVN.Service.AdminAPI.Validators.Settings
{
    [UsedImplicitly]

    public class OperationFeesRequestValidator : AbstractValidator<OperationFeesModel>
    {
        public OperationFeesRequestValidator()
        {
            RuleFor(m => m.CrossChainTransferFee)
                .GreaterThanOrEqualTo(0m)
                .WithMessage("The amount of tokens should be equal or greater than 0.");

            RuleFor(m => m.FirstTimeLinkingFee)
                .GreaterThanOrEqualTo(0m)
                .WithMessage("The amount of tokens should be equal or greater than 0.");

            RuleFor(m => m.SubsequentLinkingFee)
                .GreaterThanOrEqualTo(0m)
                .WithMessage("The amount of tokens should be equal or greater than 0.");
        }
    }
}
