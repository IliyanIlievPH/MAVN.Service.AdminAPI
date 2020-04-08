namespace MAVN.Service.AdminAPI.Models.Customers
{
    /// <summary>
    /// The response from the public wallet address request
    /// </summary>
    public class CustomerPrivateWalletAddressResponse
    {
        /// <summary>
        /// The address of the wallet
        /// </summary>
        public string WalletAddress { get; set; }
    }
}
