using JetBrains.Annotations;

namespace MAVN.Service.AdminAPI.Models.Locations
{
    /// <summary>
    /// 
    /// </summary>
    [PublicAPI]
    public class LocationModel
    {
        /// <summary>
        /// The location's name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The location's address
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// The location's contact first name.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// The location's contact last name.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// The location's contact phone.
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// The location's contact email.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// The location's external identifier.
        /// </summary>
        public string ExternalId { get; set; }

        /// <summary>
        /// Accounting integration code
        /// </summary>
        public string AccountingIntegrationCode { get; set; }
    }
}
