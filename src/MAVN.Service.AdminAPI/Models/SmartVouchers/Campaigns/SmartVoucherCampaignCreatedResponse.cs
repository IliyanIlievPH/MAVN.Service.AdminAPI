using System;
using System.Collections.Generic;
using MAVN.Service.AdminAPI.Models.ActionRules;

namespace MAVN.Service.AdminAPI.Models.SmartVouchers.Campaigns
{
    /// <summary>
    /// Represents and response model for the burn rule create request
    /// </summary>
    public class SmartVoucherCampaignCreatedResponse
    {
        /// <summary>
        /// Represents the id of the burn rule created
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Represents the image contents created for the burn rule
        /// </summary>
        public IEnumerable<ImageContentCreatedResponse> CreatedImageContents { get; set; }
    }
}
