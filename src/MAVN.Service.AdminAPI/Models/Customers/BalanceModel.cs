using MAVN.Numerics;
using JetBrains.Annotations;

namespace MAVN.Service.AdminAPI.Models.Customers
{
    /// <summary>
    /// Represents a balance information. 
    /// </summary>
    [PublicAPI]
    public class BalanceModel
    {
        /// <summary>
        /// The asset name.
        /// </summary>
        public string Asset { get; set; }

        /// <summary>
        /// The amount of asset.
        /// </summary>
        public Money18? Amount { get; set; }
    }
}
