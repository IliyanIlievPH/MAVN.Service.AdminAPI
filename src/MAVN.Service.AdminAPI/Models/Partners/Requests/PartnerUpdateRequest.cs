using System;
using System.Collections.Generic;
using MAVN.Service.AdminAPI.Models.Locations.Requests;

namespace MAVN.Service.AdminAPI.Models.Partners.Requests
{
    /// <summary>
    /// Represents a partner update information.
    /// </summary>
    public class PartnerUpdateRequest : PartnerBaseModel
    {
        /// <summary>
        /// The partner identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// List with partner's locations
        /// </summary>
        public IReadOnlyList<LocationEditRequest> Locations { get; set; }
    }
}
