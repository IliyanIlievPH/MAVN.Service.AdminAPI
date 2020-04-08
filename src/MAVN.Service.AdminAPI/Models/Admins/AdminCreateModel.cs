using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace MAVN.Service.AdminAPI.Models.Admins
{
    /// <summary>
    /// Represents an admin
    /// </summary>
    [PublicAPI]
    public class AdminCreateModel : AdminEditBaseModel
    {
        /// <summary>
        /// The admin email address.
        /// </summary>
        [Required]
        public string Email { get; set; }
        
        /// <summary>
        /// The admin password.
        /// </summary>
        [Required]
        public string Password { get; set; }
    }
}
