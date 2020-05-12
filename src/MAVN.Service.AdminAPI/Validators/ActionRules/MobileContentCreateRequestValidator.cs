using FluentValidation;
using MAVN.Service.Campaign.Client.Models.Enums;
using MAVN.Service.AdminAPI.Models.ActionRules;

namespace MAVN.Service.AdminAPI.Validators.ActionRules
{
    public class MobileContentCreateRequestValidator<T> : AbstractValidator<T> where T : MobileContentCreateRequest
    {
        public MobileContentCreateRequestValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            When(r => r.MobileLanguage.Equals(Localization.En), () =>
            {
                RuleFor(o => o.Title)
                    .NotEmpty()
                    .WithMessage("Title is required");

                RuleFor(o => o.Description)
                    .NotEmpty()
                    .WithMessage("Description is required");
            });

            RuleFor(o => o.Title)
                .Must(x => string.IsNullOrEmpty(x) || (x.Length >= 3 && x.Length <= 50))
                .WithMessage("Title length should be between 3 and 50 characters");

            RuleFor(o => o.Description)
                .Must(x => string.IsNullOrEmpty(x) || (x.Length >= 3 && x.Length <= 1000))
                .WithMessage("Description length should be between 3 and 1000 characters");
        }
    }
}
