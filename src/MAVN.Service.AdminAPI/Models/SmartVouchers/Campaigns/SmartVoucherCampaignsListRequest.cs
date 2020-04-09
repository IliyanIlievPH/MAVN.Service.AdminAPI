using MAVN.Service.AdminAPI.Models.Common;

namespace MAVN.Service.AdminAPI.Models.SmartVouchers.Campaigns
{
    public class SmartVoucherCampaignsListRequest : PagedRequestModel
    {
        public string CampaignName { get; set; }
        public bool OnlyActive { get; set; }
    }
}
