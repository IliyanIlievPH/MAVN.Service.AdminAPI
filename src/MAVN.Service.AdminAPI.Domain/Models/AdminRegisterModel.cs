using MAVN.Service.AdminAPI.Domain.Enums;

namespace MAVN.Service.AdminAPI.Domain.Models
{
    public class AdminRegisterModel
    {
        public string CompanyName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public AdminLocalization Localization { get; set; }
    }
}
