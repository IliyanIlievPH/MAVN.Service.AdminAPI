using System;
using System.ComponentModel.DataAnnotations;

namespace MAVN.Service.AdminAPI.Models.Admins
{
    public class AdminUpdateModel : AdminEditBaseModel
    {
        /// <summary>
        /// Admin user id
        /// </summary>
        [Required]
        public Guid Id { get; set; }

        /// <summary>
        /// Is admin active or not.
        /// </summary>
        [Required]
        public bool IsActive { get; set; }
    }
}
