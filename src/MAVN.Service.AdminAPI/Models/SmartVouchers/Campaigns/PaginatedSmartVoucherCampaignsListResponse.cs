using System.Collections.Generic;
using MAVN.Service.AdminAPI.Models.Common;

namespace MAVN.Service.AdminAPI.Models.SmartVouchers.Campaigns
{
    public class PaginatedSmartVoucherCampaignsListResponse
    {
        public PagedResponseModel PagedResponse { get; set; }

        public List<SmartVoucherCampaignResponse> SmartVoucherCampaigns { get; set; }
    }
}
