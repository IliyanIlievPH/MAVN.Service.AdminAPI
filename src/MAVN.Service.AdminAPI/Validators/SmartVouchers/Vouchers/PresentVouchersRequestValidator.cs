using FluentValidation;
using MAVN.Service.AdminAPI.Models.SmartVouchers.Vouchers;

namespace MAVN.Service.AdminAPI.Validators.SmartVouchers.Vouchers
{
    public class PresentVouchersRequestValidator : AbstractValidator<PresentVouchersRequest>
    {
        public PresentVouchersRequestValidator()
        {
            RuleForEach(x => x.CustomersEmails)
                .NotNull()
                .NotEmpty()
                .EmailAddress()
                .WithMessage("All emails must be valid");
        }
    }
}
