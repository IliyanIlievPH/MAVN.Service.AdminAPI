using System;
using JetBrains.Annotations;

namespace MAVN.Service.AdminAPI.Models.Tiers
{
    /// <summary>
    /// Represents the information of tier and number of customers associated with this tier.
    /// </summary>
    [PublicAPI]
    public class TierModel
    {
        /// <summary>
        /// The unique identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The tier display name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The minimum amount of tokens required for this reward tier.
        /// </summary>
        public int Threshold { get; set; }

        /// <summary>
        /// The of number of customers associated with this tier.
        /// </summary>
        public int CustomersCount { get; set; }
    }
}
