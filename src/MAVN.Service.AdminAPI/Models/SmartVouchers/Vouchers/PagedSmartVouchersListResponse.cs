using System.Collections.Generic;
using MAVN.Service.AdminAPI.Models.Common;

namespace MAVN.Service.AdminAPI.Models.SmartVouchers.Vouchers
{
    public class PagedSmartVouchersListResponse
    {
        public PagedResponseModel PagedResponse { get; set; }
        public List<SmartVoucherResponse> Vouchers { get; set; }
    }
}
