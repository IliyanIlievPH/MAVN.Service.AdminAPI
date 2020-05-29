using FluentValidation;
using MAVN.Service.AdminAPI.Models.Emails;

namespace MAVN.Service.AdminAPI.Validators.Emails
{
    public class EmailVerificationRequestValidator : AbstractValidator<EmailVerificationRequest>
    {
        public EmailVerificationRequestValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(m => m.VerificationCode)
                .NotEmpty()
                .WithMessage("VerificationCode is required");
        }
    }
}
