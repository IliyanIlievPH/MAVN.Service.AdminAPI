using Common;
using FluentValidation;
using MAVN.Service.AdminAPI.Models.ActionRules;

namespace MAVN.Service.AdminAPI.Validators.ActionRules
{
    public class ImageEditRequestValidator : AbstractValidator<ImageEditRequest>
    {
        public ImageEditRequestValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(o => o.Id)
                .NotNull()
                .NotEmpty()
                .WithMessage("Id should be specified")
                .Must(o => o.IsGuid())
                .WithMessage("Id should be guid");

            RuleFor(o => o.RuleContentId)
                .NotNull()
                .NotEmpty()
                .WithMessage("RuleContentId should be specified");
        }
    }
}
