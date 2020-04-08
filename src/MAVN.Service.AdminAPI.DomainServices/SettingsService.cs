using MAVN.Service.AdminAPI.Domain.Services;

namespace MAVN.Service.AdminAPI.DomainServices
{
    public class SettingsService : ISettingsService
    {
        private readonly string _tokenName;
        private readonly bool _isPublicBlockchainFeatureDisabled;

        public SettingsService(
            string tokenName,
            bool? isPublicBlockchainFeatureDisabled)
        {
            _tokenName = tokenName;
            _isPublicBlockchainFeatureDisabled = isPublicBlockchainFeatureDisabled ?? false;
        }

        public string GetTokenName() => _tokenName;

        public bool IsPublicBlockchainFeatureDisabled() => _isPublicBlockchainFeatureDisabled;
    }
}
