using FluentValidation;
using JetBrains.Annotations;
using MAVN.Service.AdminAPI.Models.Partners.Requests;
using MAVN.Service.AdminAPI.Validators.Locations;

namespace MAVN.Service.AdminAPI.Validators.Partners
{
    [UsedImplicitly]
    public class PartnerUpdateModelValidator : PartnerBaseModelValidator<PartnerUpdateRequest>
    {
        public PartnerUpdateModelValidator()
        {
            RuleFor(o => o.Id)
                .NotEmpty()
                .WithMessage("Id required");

            RuleFor(p => p.Locations)
                .NotEmpty()
                .WithMessage("Location required");

            RuleForEach(p => p.Locations)
                .SetValidator(new LocationEditRequestValidator());

            RuleFor(p => p.Locations)
                .Must(p => p != null && p.Count > 0)
                .WithMessage("The Partner should have at least one location.");

            RuleFor(p => p.ClientSecret)
                .MinimumLength(6)
                .MaximumLength(64)
                .WithMessage("The Client Secret should be empty or within range of 6 to 64 characters long.");

            RuleFor(p => p.ClientId)
                .MinimumLength(6)
                .MaximumLength(64)
                .WithMessage("The Client Id should be empty or within a range of 6 to 64 characters long.")
                .Must((p, c) => p != null && (!string.IsNullOrEmpty(p.ClientId) || string.IsNullOrEmpty(p.ClientSecret)))
                .WithMessage("When changing the client id, a client secret is required.");
        }
    }
}
