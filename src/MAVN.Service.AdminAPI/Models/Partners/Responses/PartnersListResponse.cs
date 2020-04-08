using System.Collections.Generic;
using JetBrains.Annotations;
using MAVN.Service.AdminAPI.Models.Common;

namespace MAVN.Service.AdminAPI.Models.Partners.Responses
{
    /// <summary>
    /// Represents a partners list response.
    /// </summary>
    [PublicAPI]
    public class PartnersListResponse
    {
        /// <summary>
        /// Paging model
        /// </summary>
        public PagedResponseModel PagedResponse { get; set; }

        /// <summary>
        /// List of partners
        /// </summary>
        public IEnumerable<PartnerRowResponse> Partners { get; set; }
    }
}
