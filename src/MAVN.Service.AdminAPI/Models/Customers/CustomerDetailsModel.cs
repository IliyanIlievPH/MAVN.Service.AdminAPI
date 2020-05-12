using JetBrains.Annotations;
using MAVN.Service.AdminAPI.Models.Customers.Enums;

namespace MAVN.Service.AdminAPI.Models.Customers
{
    /// <summary>
    /// Represents a customer
    /// </summary>
    [PublicAPI]
    public class CustomerDetailsModel : CustomerModel
    {
        /// <summary>
        /// Represents wallet status
        /// </summary>
        public CustomerWalletActivityStatus WalletStatus { get; set; }
    }
}
