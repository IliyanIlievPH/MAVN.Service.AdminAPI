using System.Text.RegularExpressions;
using FluentValidation;
using MAVN.Service.AdminAPI.Models.Locations;

namespace MAVN.Service.AdminAPI.Validators.Locations
{
    public class LocationModelValidator<T> : AbstractValidator<T> where T : LocationModel
    {
        private readonly Regex _phoneNumberRegex = new Regex(@"^[0-9 A-Z a-z #;,()+*-]{1,30}$");
        private readonly Regex _onlyLettersRegex =new Regex(@"^((?![1-9!@#$%^&*()_+{}|:\""?></,;[\]\\=~]).)+$");

        public LocationModelValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(o => o.Name)
                .NotEmpty()
                .WithMessage("Name required")
                .Length(3, 100)
                .WithMessage("Name should be between 3 and 100 chars");

            RuleFor(o => o.Address)
                .NotEmpty()
                .WithMessage("Address required")
                .Length(3, 100)
                .WithMessage("Address should be between 3 and 100 chars");

            RuleFor(o => o.FirstName)
                .NotEmpty()
                .WithMessage("First Name required")
                .Must(o => o != null && _onlyLettersRegex.IsMatch(o))
                .WithMessage("First Name should contains only letters")
                .Length(2, 50)
                .WithMessage("First Name should be between 2 and 50 chars")
                .When(x => !string.IsNullOrEmpty(x.LastName) || !string.IsNullOrEmpty(x.Email) || !string.IsNullOrEmpty(x.Phone));

            RuleFor(o => o.LastName)
                .NotEmpty()
                .WithMessage("Last Name required")
                .Must(o => o != null && _onlyLettersRegex.IsMatch(o))
                .WithMessage("Last Name should contains only letters")
                .Length(2, 50)
                .WithMessage("Last Name should be between 2 and 50 chars")
                .When(x => !string.IsNullOrEmpty(x.FirstName) || !string.IsNullOrEmpty(x.Email) || !string.IsNullOrEmpty(x.Phone));

            RuleFor(o => o.Email)
                .NotEmpty()
                .WithMessage("Email required")
                .EmailAddress()
                .WithMessage("Please enter a valid email address")
                .When(x => !string.IsNullOrEmpty(x.LastName) || !string.IsNullOrEmpty(x.FirstName) || !string.IsNullOrEmpty(x.Phone));

            RuleFor(o => o.Phone)
                .NotEmpty()
                .WithMessage("Phone number required.")
                .MinimumLength(3)
                .MaximumLength(50)
                .WithMessage("Phone number should be within 3 and 50 characters long.")
                .Must(o => o != null && _phoneNumberRegex.IsMatch(o))
                .WithMessage("Phone number contains illegal characters.")
                .When(x => !string.IsNullOrEmpty(x.LastName) || !string.IsNullOrEmpty(x.Email) || !string.IsNullOrEmpty(x.FirstName));

            RuleFor(l => l.ExternalId)
                .NotEmpty()
                .MinimumLength(1)
                .MaximumLength(80)
                .WithMessage("The external id should be within a range of 1 to 80 characters long.");

            RuleFor(x => x.AccountingIntegrationCode)
                .NotNull()
                .NotEmpty()
                .MinimumLength(1)
                .MaximumLength(80)
                .WithMessage("The accounting integration code should be within a range of 1 to 80 characters long.");
        }
    }
}
