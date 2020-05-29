using MAVN.Service.AdminAPI.Domain.Services;

namespace MAVN.Service.AdminAPI.DomainServices
{
    public class SettingsService : ISettingsService
    {
        private readonly string _tokenName;
        private readonly bool _isPublicBlockchainFeatureDisabled;
        private readonly bool _isPhoneVerificationDisabled;

        public SettingsService(
            string tokenName,
            bool isPublicBlockchainFeatureDisabled,
            bool isPhoneVerificationDisabled)
        {
            _tokenName = tokenName;
            _isPublicBlockchainFeatureDisabled = isPublicBlockchainFeatureDisabled;
            _isPhoneVerificationDisabled = isPhoneVerificationDisabled;
        }

        public string GetTokenName() => _tokenName;

        public bool IsPublicBlockchainFeatureDisabled() => _isPublicBlockchainFeatureDisabled;
        public bool IsPhoneVerificationDisabled() => _isPhoneVerificationDisabled;
    }
}
