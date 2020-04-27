using JetBrains.Annotations;
using MAVN.Service.AdminAPI.Domain.Models;

namespace MAVN.Service.AdminAPI.Models.Auth
{
    [PublicAPI]
    public class LoginResponseModel
    {
        public string Token { get; set; }
        public AdminModel AdminUser { get; set; }
    }
}
