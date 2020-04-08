using Common;
using FluentValidation;
using JetBrains.Annotations;
using MAVN.Service.AdminAPI.Models.Customers;

namespace MAVN.Service.AdminAPI.Validators.Customers
{
    [UsedImplicitly]
    public class CustomerOperationsHistoryRequestValidator : AbstractValidator<CustomerOperationsHistoryRequest>
    {
        public CustomerOperationsHistoryRequestValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(x => x.CustomerId)
                .NotEmpty()
                .WithMessage(x => $"{nameof(x.CustomerId)} required")
                .Must(x => x.IsGuid())
                .WithMessage(x => $"{nameof(x.CustomerId)} has invalid format");
        }
    }
}
