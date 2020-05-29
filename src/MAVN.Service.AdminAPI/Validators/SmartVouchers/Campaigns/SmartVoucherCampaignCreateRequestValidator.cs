using FluentValidation;
using MAVN.Service.AdminAPI.Models.ActionRules;
using MAVN.Service.AdminAPI.Models.SmartVouchers.Campaigns;
using MAVN.Service.AdminAPI.Validators.ActionRules;

namespace MAVN.Service.AdminAPI.Validators.SmartVouchers.Campaigns
{
    public class SmartVoucherCampaignCreateRequestValidator : SmartVoucherCampaignBaseRequestValidator<SmartVoucherCampaignCreateRequest, MobileContentCreateRequest>
    {
        public SmartVoucherCampaignCreateRequestValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            When(o => o.MobileContents != null, () =>
            {
                RuleForEach(o => o.MobileContents)
                    .SetValidator(new MobileContentCreateRequestValidator<MobileContentCreateRequest>());
            });
        }
    }
}
