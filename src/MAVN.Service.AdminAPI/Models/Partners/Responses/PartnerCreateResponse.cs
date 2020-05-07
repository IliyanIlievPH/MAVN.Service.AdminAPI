using JetBrains.Annotations;

namespace MAVN.Service.AdminAPI.Models.Partners.Responses
{
    /// <summary>
    /// Response model for partner creation
    /// </summary>
    [PublicAPI]
    public class PartnerCreateResponse
    {
        /// <summary>
        /// Id of the partner
        /// </summary>
        public string PartnerId { get; set; }
    }
}
