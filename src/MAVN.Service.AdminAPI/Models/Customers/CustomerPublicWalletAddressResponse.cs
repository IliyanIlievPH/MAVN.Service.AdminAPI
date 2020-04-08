using MAVN.Service.AdminAPI.Models.Customers.Enums;

namespace MAVN.Service.AdminAPI.Models.Customers
{
    /// <summary>
    /// The response from the public wallet address request
    /// </summary>
    public class CustomerPublicWalletAddressResponse
    {
        /// <summary>
        /// The address of the wallet
        /// </summary>
        public string PublicAddress { get; set; }
        
        /// <summary>
        /// The status of the wallet
        /// </summary>
        public PublicAddressStatus Status { get; set; }
    }
}
