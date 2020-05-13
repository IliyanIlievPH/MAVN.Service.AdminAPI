namespace MAVN.Service.AdminAPI.Models.Partners.Responses
{
    /// <summary>
    /// Response model for partner ability check
    /// </summary>
    public class CheckPartnerAbilityResponse
    {
        /// <summary>
        /// Boolean if the partner has ability
        /// </summary>
        public bool HasAbility { get; set; }
        /// <summary>
        /// The reason for the inability
        /// </summary>
        public string InabilityReason { get; set; }
    }
}
