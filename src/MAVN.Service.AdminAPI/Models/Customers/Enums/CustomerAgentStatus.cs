using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MAVN.Service.AdminAPI.Models.Customers.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CustomerAgentStatus
    {
        /// <summary>
        /// Indicates that the customer is not an agent.
        /// </summary>
        NotAgent,

        /// <summary>
        /// Indicates that the customer registration as an agent was rejected.
        /// </summary>
        Rejected,

        /// <summary>
        /// Indicates that the customer registered as an agent and successfully complete KYA.
        /// </summary>
        ApprovedAgent
    }
}
