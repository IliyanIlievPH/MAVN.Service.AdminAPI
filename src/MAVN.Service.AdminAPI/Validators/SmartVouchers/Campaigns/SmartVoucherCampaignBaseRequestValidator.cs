using System.Linq;
using Common;
using FluentValidation;
using MAVN.Service.AdminAPI.Interfaces.ActionRules;
using MAVN.Service.AdminAPI.Models.ActionRules;
using MAVN.Service.AdminAPI.Models.SmartVouchers.Campaigns;

namespace MAVN.Service.AdminAPI.Validators.SmartVouchers.Campaigns
{
    public abstract class SmartVoucherCampaignBaseRequestValidator<T, TU> : AbstractValidator<T>
        where T : SmartVoucherCampaignBaseRequest<TU> where TU : IMobileContentRequest
    {
        protected SmartVoucherCampaignBaseRequestValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(o => o.Name)
                .NotNull()
                .NotEmpty()
                .MinimumLength(3)
                .MaximumLength(50)
                .WithMessage("Name should be specified with length between 3 and 50 characters");

            RuleFor(o => o.Description)
                .Must(x => string.IsNullOrEmpty(x) || (x.Length >= 3 && x.Length <= 1000))
                .WithMessage("Description should be specified with length between 3 and 1000 characters");

            RuleFor(o => o.PartnerId)
                .NotNull()
                .NotEmpty()
                .WithMessage("PartnerId should be specified")
                .Must(o => o.IsGuid())
                .WithMessage("PartnerId should be guid");

            RuleFor(o => o.MobileContents)
                .Must(contents => contents != null && contents.Any())
                .WithMessage(o => $"There should be at least one item in the {nameof(o.MobileContents)} value")
                .Must(contents => { return contents.Any(c => c.MobileLanguage == MobileLocalization.En); })
                .WithMessage("English content is required.");
        }
    }
}
