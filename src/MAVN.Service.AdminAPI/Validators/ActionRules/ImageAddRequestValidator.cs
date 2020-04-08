using FluentValidation;
using MAVN.Service.AdminAPI.Models.ActionRules;

namespace MAVN.Service.AdminAPI.Validators.ActionRules
{
    public class ImageAddRequestValidator: AbstractValidator<ImageAddRequest>
    {
        public ImageAddRequestValidator()
        {
            RuleFor(o => o.RuleContentId)
                .NotNull()
                .NotEmpty()
                .WithMessage("RuleContentId should be specified");
        }
    }
}
