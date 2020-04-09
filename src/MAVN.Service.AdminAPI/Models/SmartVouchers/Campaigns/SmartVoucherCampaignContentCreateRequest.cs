namespace MAVN.Service.AdminAPI.Models.SmartVouchers.Campaigns
{
    /// <summary>
    /// Smart voucher campaign content create model
    /// </summary>
    public class SmartVoucherCampaignContentCreateRequest
    {
        /// <summary>Represents the type of the content</summary>
        public SmartVoucherCampaignContentType ContentType { get; set; }

        /// <summary>Represents content's language</summary>
        public Localization Localization { get; set; }

        /// <summary>Represents content's value</summary>
        public string Value { get; set; }
    }
}
