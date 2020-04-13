using FluentValidation;
using MAVN.Service.AdminAPI.Models.Partners;

namespace MAVN.Service.AdminAPI.Validators.Partners
{
    public abstract class PartnerBaseModelValidator<T> : AbstractValidator<T> where T : PartnerBaseModel
    {
        protected PartnerBaseModelValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(o => o.Name)
                //.NotEmpty()
                //.WithMessage("Name required")
                .Length(3, 50)
                .WithMessage("Name should be between 3 and 50 chars");

            RuleFor(p => p.AmountInTokens)
                .Must((model, value) => model.UseGlobalCurrencyRate || !model.UseGlobalCurrencyRate && value > 0)
                .WithMessage("Amount in tokens should be greater than 0.");

            RuleFor(p => p.AmountInCurrency)
                .Must((model, value) => model.UseGlobalCurrencyRate || !model.UseGlobalCurrencyRate && value > 0)
                .WithMessage("Amount in currency should be greater than 0.");

            RuleFor(p => p.BusinessVertical)
                .NotNull()
                .WithMessage("Business vertical required");

            RuleFor(p => p.Description)
                .Length(3, 1000)
                .WithMessage("Description should be between 3 and 1000 chars");

            RuleFor(o => o.ClientId)
                .NotEmpty()
                .WithMessage("ClientId required");
        }
    }
}
