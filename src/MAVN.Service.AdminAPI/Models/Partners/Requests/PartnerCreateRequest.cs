using System.Collections.Generic;
using MAVN.Service.AdminAPI.Models.Locations.Requests;

namespace MAVN.Service.AdminAPI.Models.Partners.Requests
{
    /// <summary>
    /// Represent partner creation information.
    /// </summary>
    public class PartnerCreateRequest : PartnerBaseModel
    {
        /// <summary>
        /// List with partner's locations
        /// </summary>
        public IReadOnlyList<LocationCreateRequest> Locations { get; set; }
    }
}
