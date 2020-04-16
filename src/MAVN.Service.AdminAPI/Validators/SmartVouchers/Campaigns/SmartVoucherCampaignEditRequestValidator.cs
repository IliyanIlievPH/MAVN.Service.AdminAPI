using FluentValidation;
using MAVN.Service.AdminAPI.Models.ActionRules;
using MAVN.Service.AdminAPI.Models.SmartVouchers.Campaigns;
using MAVN.Service.AdminAPI.Validators.ActionRules;

namespace MAVN.Service.AdminAPI.Validators.SmartVouchers.Campaigns
{
    public class SmartVoucherCampaignEditRequestValidator : SmartVoucherCampaignBaseRequestValidator<SmartVoucherCampaignEditRequest, MobileContentEditRequest>
    {
        public SmartVoucherCampaignEditRequestValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(o => o.Id)
                .NotEmpty()
                .WithMessage("Id required");

            When(o => o.MobileContents != null, () =>
            {
                RuleForEach(o => o.MobileContents)
                    .SetValidator(new MobileContentEditRequestValidator());
            });
        }
    }
}
