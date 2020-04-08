using System;
using FluentValidation;
using MAVN.Service.AdminAPI.Models.ActionRules;

namespace MAVN.Service.AdminAPI.Validators.ActionRules
{
    public class ConversionRateValidator : AbstractValidator<IConversionRateHolder>
    {
        public ConversionRateValidator()
        {
            RuleFor(x => x.AmountInTokens)
                .Must((model, value) => model != null &&
                                        ValidateConversionRate(model.UsePartnerCurrencyRate, () => value.HasValue))
                .WithMessage("Amount in tokens required.")
                .Must((model, value) => model != null &&
                                        ValidateConversionRate(model.UsePartnerCurrencyRate,
                                            () => value.HasValue && value.Value > 0))
                .WithMessage("Amount in tokens should be greater than 0.");

            RuleFor(x => x.AmountInCurrency)
                .Must((model, value) => model != null &&
                                        ValidateConversionRate(model.UsePartnerCurrencyRate, () => value.HasValue))
                .WithMessage("Amount in currency required.")
                .Must((model, value) => model != null &&
                                        ValidateConversionRate(model.UsePartnerCurrencyRate,
                                            () => value.HasValue && value.Value > 0))
                .WithMessage("Amount in currency should be greater than 0.");
        }

        private static bool ValidateConversionRate(bool usePartnerCurrencyRate, Func<bool> fn)
        {
            return usePartnerCurrencyRate || fn.Invoke();
        }
    }
}
