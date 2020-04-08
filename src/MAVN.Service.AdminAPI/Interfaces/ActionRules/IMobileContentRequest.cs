using Lykke.Service.Campaign.Client.Models.Enums;

namespace MAVN.Service.AdminAPI.Interfaces.ActionRules
{
    public interface IMobileContentRequest
    {
        Localization MobileLanguage { get; set; }
    }
}
