using System;

namespace MAVN.Service.AdminAPI.Models.Partners.Responses
{
    /// <summary>
    /// Response model which holds partner linking info
    /// </summary>
    public class PartnerLinkingInfoResponse
    {
        /// <summary>Id of the partner</summary>
        public Guid PartnerId { get; set; }

        /// <summary>Code of the partner</summary>
        public string PartnerCode { get; set; }

        /// <summary>Code used for linking of the partner</summary>
        public string PartnerLinkingCode { get; set; }
    }
}
