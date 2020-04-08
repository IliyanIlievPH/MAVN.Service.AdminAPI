using MAVN.Service.AdminAPI.Models.Common;

namespace MAVN.Service.AdminAPI.Models.Partners.Requests
{
    /// <summary>
    /// Represents a partner's list request model
    /// </summary>
    public class PartnerListRequest : PagedRequestModel
    {
        /// <summary>
        /// Partner's name filter
        /// Optional
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Partner's vertical filter
        /// Optional
        /// </summary>
        public BusinessVertical? BusinessVertical { get; set; }
    }
}
