using System;
using System.Collections.Generic;
using MAVN.Service.AdminAPI.Models.Locations.Responses;

namespace MAVN.Service.AdminAPI.Models.Partners.Responses
{
    /// <summary>
    /// Represents a partner details information.
    /// </summary>
    public class PartnerDetailsResponse : PartnerBaseModel
    {
        /// <summary>
        /// The partner identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// List with partner's locations
        /// </summary>
        public IReadOnlyList<LocationResponse> Locations { get; set; }

        /// <summary>
        /// Represents code which is used to refer customers
        /// </summary>
        public string ReferralCode { get; set; }
    }
}
