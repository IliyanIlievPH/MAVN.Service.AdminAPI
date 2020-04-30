using JetBrains.Annotations;

namespace MAVN.Service.AdminAPI.Models.PaymentProviderDetails
{
    /// <summary>
    /// Holds information about payment provider
    /// </summary>
    [PublicAPI]
    public class PaymentProviderProperty
    {
        /// <summary>Property  name</summary>
        public string Name { get; set; }

        /// <summary>Property description</summary>
        public string Description { get; set; }

        /// <summary>IsOptional property flag</summary>
        public bool IsOptional { get; set; }

        /// <summary>IsSecret property flag</summary>
        public bool IsSecret { get; set; }
    }
}
