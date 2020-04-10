namespace MAVN.Service.AdminAPI.Domain.Services
{
    public interface ISettingsService
    {
        string GetTokenName();

        bool IsPublicBlockchainFeatureDisabled();

        bool IsPhoneVerificationDisabled();
    }
}
