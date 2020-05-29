using System;
using System.ComponentModel.DataAnnotations;

namespace MAVN.Service.AdminAPI.Models.SmartVouchers.Campaigns
{
    /// <summary>
    /// Request model to get supported currencies
    /// </summary>
    public class GetSupportedCurrenciesRequest
    {
        /// <summary>
        /// Id of the partner
        /// </summary>
        [Required]
        public Guid PartnerId { get; set; }
    }
}
