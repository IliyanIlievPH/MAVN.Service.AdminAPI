using FluentValidation;
using MAVN.Service.Campaign.Client.Models.Enums;
using MAVN.Service.AdminAPI.Models.ActionRules;

namespace MAVN.Service.AdminAPI.Validators.ActionRules
{
    public class MobileContentEditRequestValidator : MobileContentCreateRequestValidator<MobileContentEditRequest>
    {
        public MobileContentEditRequestValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            When(r => r.MobileLanguage.Equals(Localization.En), () =>
            {
                RuleFor(o => o.TitleId)
                    .NotEmpty()
                    .WithMessage("TitleId should be specified");

                RuleFor(o => o.DescriptionId)
                    .NotEmpty()
                    .WithMessage("DescriptionId should be specified");
            });
        }
    }
}
