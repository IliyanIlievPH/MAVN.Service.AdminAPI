using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace MAVN.Service.AdminAPI.Domain.Models
{
    /// <summary>
    /// Represents an admin
    /// </summary>
    [PublicAPI]
    public class AdminModel
    {
        /// <summary>
        /// The admin identifier.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Is admin active or not.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// The admin email address.
        /// </summary>
        public string Email { get; set; }

        /// <summary>Email Verified flag.</summary>
        public string IsEmailVerified { get; set; }

        /// <summary>
        /// The admin first name.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// The admin last name.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// The registered date.
        /// </summary>
        public DateTime Registered { get; set; }

        /// <summary>
        /// Phone number
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Company
        /// </summary>
        public string Company { get; set; }

        /// <summary>
        /// Department
        /// </summary>
        public string Department { get; set; }

        /// <summary>
        /// Job Title
        /// </summary>
        public string JobTitle { get; set; }

        /// <summary>
        /// Permissions
        /// </summary>
        public List<Permission> Permissions { set; get; }
    }
}
