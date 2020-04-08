using JetBrains.Annotations;
using MAVN.Service.AdminAPI.Models.Admins;

namespace MAVN.Service.AdminAPI.Models.Auth
{
    [PublicAPI]
    public class LoginResponseModel
    {
        public string Token { get; set; }
        public AdminModel AdminUser { get; set; }
    }
}
