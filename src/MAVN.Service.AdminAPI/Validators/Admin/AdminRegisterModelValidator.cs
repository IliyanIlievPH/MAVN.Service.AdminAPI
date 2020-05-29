using FluentValidation;
using MAVN.Service.AdminAPI.Domain.Models;

namespace MAVN.Service.AdminAPI.Validators.Admin
{
    public class AdminRegisterModelValidator : AbstractValidator<AdminRegisterModel>
    {
        public AdminRegisterModelValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            // see PartnerBaseModelValidator
            RuleFor(o => o.CompanyName)
                .Length(3, 50)
                .WithMessage("CompanyName should be between 3 and 50 chars");

            RuleFor(o => o.Email)
                .NotEmpty()
                .WithMessage("Email required")
                .EmailAddress()
                .WithMessage("Please enter a valid email address");

            RuleFor(o => o.Password)
                .NotEmpty()
                .WithMessage("Password required");
        }
    }
}
