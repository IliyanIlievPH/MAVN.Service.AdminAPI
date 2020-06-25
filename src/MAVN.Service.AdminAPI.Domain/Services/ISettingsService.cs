namespace MAVN.Service.AdminAPI.Domain.Services
{
    public interface ISettingsService
    {
        string GetTokenName();
        string GetBaseCurrency();
        bool IsPublicBlockchainFeatureDisabled();
        bool IsPhoneVerificationDisabled();
        string GetReferralUrlTemplate();
        bool IsDemoOn();
    }
}
