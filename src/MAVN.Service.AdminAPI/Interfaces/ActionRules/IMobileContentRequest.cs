using MAVN.Service.AdminAPI.Models.ActionRules;

namespace MAVN.Service.AdminAPI.Interfaces.ActionRules
{
    public interface IMobileContentRequest
    {
        MobileLocalization MobileLanguage { get; set; }
    }
}
