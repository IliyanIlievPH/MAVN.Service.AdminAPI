using FluentValidation;
using JetBrains.Annotations;
using MAVN.Service.AdminAPI.Models.Settings;

namespace MAVN.Service.AdminAPI.Validators.Settings
{
    [UsedImplicitly]
    public class GlobalCurrencyRateValidator : AbstractValidator<GlobalCurrencyRateModel>
    {
        public GlobalCurrencyRateValidator()
        {
            RuleFor(o => o.AmountInTokens)
                .GreaterThan(0)
                .WithMessage("Amount in tokens should be greater than 0.");

            RuleFor(o => o.AmountInCurrency)
                .GreaterThan(0)
                .WithMessage("Amount in currency should be greater than 0.");
        }
    }
}
