using FluentValidation;
using MAVN.Service.AdminAPI.Models.PaymentProviderDetails;

namespace MAVN.Service.AdminAPI.Validators.PaymentProviderDetails
{
    public class CheckPaymentIntegrationRequestValidator : AbstractValidator<CheckPaymentIntegrationRequest>
    {
        public CheckPaymentIntegrationRequestValidator()
        {
            RuleFor(x => x.PartnerId)
                .Must((model, value) =>
                    model != null && string.IsNullOrEmpty(model.PaymentIntegrationProvider) ? value != default : true)
                .WithMessage(x => $"{nameof(x.PartnerId)} required");
        }
    }
}
