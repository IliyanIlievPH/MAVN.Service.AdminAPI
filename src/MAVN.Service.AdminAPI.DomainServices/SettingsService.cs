using MAVN.Service.AdminAPI.Domain.Services;

namespace MAVN.Service.AdminAPI.DomainServices
{
    public class SettingsService : ISettingsService
    {
        private readonly string _tokenName;
        private readonly bool _isPublicBlockchainFeatureDisabled;
        private readonly bool _isPhoneVerificationDisabled;
        private readonly string _referralUrlTemplate;

        public SettingsService(
            string tokenName,
            bool isPublicBlockchainFeatureDisabled,
            bool isPhoneVerificationDisabled,
            string referralUrlTemplate)
        {
            _tokenName = tokenName;
            _isPublicBlockchainFeatureDisabled = isPublicBlockchainFeatureDisabled;
            _isPhoneVerificationDisabled = isPhoneVerificationDisabled;
            _referralUrlTemplate = referralUrlTemplate;
        }

        public string GetTokenName() => _tokenName;
        public bool IsPublicBlockchainFeatureDisabled() => _isPublicBlockchainFeatureDisabled;
        public bool IsPhoneVerificationDisabled() => _isPhoneVerificationDisabled;
        public string GetReferralUrlTemplate() => _referralUrlTemplate;
    }
}
