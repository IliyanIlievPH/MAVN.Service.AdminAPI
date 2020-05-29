using System;
using System.ComponentModel.DataAnnotations;

namespace MAVN.Service.AdminAPI.Models.Partners.Requests
{
    public class RegeneratePartnerLinkingInfoRequest
    {
        /// <summary>
        /// Id of the partner
        /// </summary>
        [Required]
        public Guid PartnerId { get; set; }
    }
}
