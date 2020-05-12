using System;
using JetBrains.Annotations;
using MAVN.Service.AdminAPI.Models.Customers.Enums;

namespace MAVN.Service.AdminAPI.Models.Customers
{
    /// <summary>
    /// Represents a customer
    /// </summary>
    [PublicAPI]
    public class CustomerModel
    {
        /// <summary>
        /// Customer Id
        /// </summary>
        public string CustomerId { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Email verified
        /// </summary>
        public bool IsEmailVerified { get; set; }

        /// <summary>
        /// Phone number
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Phone number verified
        /// </summary>
        public bool IsPhoneVerified { get; set; }

        /// <summary>
        /// First Name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Last Name
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Date of registration
        /// </summary>
        public DateTime RegisteredDate { get; set; }

        /// <summary>
        /// Referral Code
        /// </summary>
        [CanBeNull]
        public string ReferralCode { get; set; }

        /// <summary>
        /// Represents customer's status 
        /// </summary>
        public CustomerActivityStatus CustomerStatus { get; set; }
    }
}
