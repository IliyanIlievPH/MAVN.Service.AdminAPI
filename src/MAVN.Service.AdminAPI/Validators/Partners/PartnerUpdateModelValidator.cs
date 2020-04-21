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
        }
    }
}
