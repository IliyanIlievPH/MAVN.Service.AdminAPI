using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace MAVN.Service.AdminAPI.Models.Auth
{
    [PublicAPI]
    public class ChangePasswordAnonymousModel : ChangePasswordModel
    {
        [Required]
        public string Email { get; set; }
    }
}
