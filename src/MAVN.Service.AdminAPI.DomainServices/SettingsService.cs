using MAVN.Service.AdminAPI.Domain.Services;

namespace MAVN.Service.AdminAPI.DomainServices
{
    public class SettingsService : ISettingsService
    {
        private readonly string _tokenName;
        private readonly bool _isPublicBlockchainFeatureDisabled;
        private readonly bool _isPhoneVerificationDisabled;
        private readonly string _referralUrlTemplate;
        private readonly string _baseCurrency;

        public SettingsService(
            string tokenName,
            bool isPublicBlockchainFeatureDisabled,
            bool isPhoneVerificationDisabled,
            string referralUrlTemplate,
            string baseCurrency)
        {
            _tokenName = tokenName;
            _isPublicBlockchainFeatureDisabled = isPublicBlockchainFeatureDisabled;
            _isPhoneVerificationDisabled = isPhoneVerificationDisabled;
            _referralUrlTemplate = referralUrlTemplate;
            _baseCurrency = baseCurrency;
        }

        public string GetTokenName() => _tokenName;
        public string GetBaseCurrency() => _baseCurrency;
        public bool IsPublicBlockchainFeatureDisabled() => _isPublicBlockchainFeatureDisabled;
        public bool IsPhoneVerificationDisabled() => _isPhoneVerificationDisabled;
        public string GetReferralUrlTemplate() => _referralUrlTemplate;
    }
}
