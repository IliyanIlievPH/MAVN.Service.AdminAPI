using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MAVN.Service.AdminAPI.Models.Customers.Enums
{
    /// <summary>
    /// Public wallet address
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum PublicAddressStatus
    {
        /// <summary>
        /// The wallet is not linked
        /// </summary>
        NotLinked,
        /// <summary>
        /// The wallet linking is pending customer approval
        /// </summary>
        PendingCustomerApproval,
        /// <summary>
        /// The linking process is pending confirmation in blockchain
        /// </summary>
        PendingConfirmation,
        /// <summary>
        /// The wallet is linked
        /// </summary>
        Linked
    }
}
