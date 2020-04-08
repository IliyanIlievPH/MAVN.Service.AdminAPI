using System.ComponentModel.DataAnnotations;

namespace MAVN.Service.AdminAPI.Models.Admins
{
    public class AdminEditBaseModel
    {
        /// <summary>
        /// The admin first name.
        /// </summary>
        [Required]
        [StringLength(255)]
        public string FirstName { get; set; }

        /// <summary>
        /// The admin last name.
        /// </summary>
        [Required]
        [StringLength(255)]
        public string LastName { get; set; }
        
        /// <summary>
        /// The admin Phone number.
        /// </summary>
        [Required]
        [StringLength(255)]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// The admin Company.
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Company { get; set; }

        /// <summary>
        /// The admin Department.
        /// </summary>
        [Required]
        [StringLength(25)]
        public string Department { get; set; }

        /// <summary>
        /// The admin Job title.
        /// </summary>
        [Required]
        [StringLength(255)]
        public string JobTitle { get; set; }
    }
}
