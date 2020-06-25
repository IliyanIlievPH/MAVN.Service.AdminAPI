using Autofac;
using MAVN.Service.AdminAPI.Domain.Services;

namespace MAVN.Service.AdminAPI.DomainServices
{
    public class AutofacModule : Module
    {
        private readonly string _tokenName;
        private readonly bool _isPublicBlockchainFeatureDisabled;
        private readonly bool _mobileAppImageDoOptimization;
        private readonly int _mobileAppImageMinWidth;
        private readonly int _mobileAppImageWarningFileSizeInKB;
        private readonly int _suggestedAdminPasswordLength;
        private readonly bool _isPhoneVerificationDisabled;
        private readonly string _referralUrlTemplate;
        private readonly string _baseCurrency;

        public AutofacModule(
            string tokenName,
            bool isPublicBlockchainFeatureDisabled,
            bool mobileAppImageDoOptimization,
            int mobileAppImageMinWidth,
            int mobileAppImageWarningFileSizeInKB,
            int suggestedAdminPasswordLength,
            bool isPhoneVerificationDisabled,
            string referralUrlTemplate,
            string baseCurrency)
        {
            _tokenName = tokenName;
            _isPublicBlockchainFeatureDisabled = isPublicBlockchainFeatureDisabled;
            _mobileAppImageDoOptimization = mobileAppImageDoOptimization;
            _mobileAppImageMinWidth = mobileAppImageMinWidth;
            _mobileAppImageWarningFileSizeInKB = mobileAppImageWarningFileSizeInKB;
            _suggestedAdminPasswordLength = suggestedAdminPasswordLength;
            _isPhoneVerificationDisabled = isPhoneVerificationDisabled;
            _referralUrlTemplate = referralUrlTemplate;
            _baseCurrency = baseCurrency;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<SettingsService>()
                .WithParameter("tokenName", _tokenName)
                .WithParameter("isPublicBlockchainFeatureDisabled", _isPublicBlockchainFeatureDisabled)
                .WithParameter("isPhoneVerificationDisabled", _isPhoneVerificationDisabled)
                .WithParameter("referralUrlTemplate", _referralUrlTemplate)
                .WithParameter("baseCurrency", _baseCurrency)
                .As<ISettingsService>();

            builder.RegisterType<CredentialsGeneratorService>()
                .WithParameter(TypedParameter.From(_suggestedAdminPasswordLength))
                .As<ICredentialsGeneratorService>();

            builder.RegisterType<ImageService>()
                .WithParameter(nameof(_mobileAppImageDoOptimization).TrimStart('_'), _mobileAppImageDoOptimization)
                .WithParameter(nameof(_mobileAppImageMinWidth).TrimStart('_'), _mobileAppImageMinWidth)
                .WithParameter(nameof(_mobileAppImageWarningFileSizeInKB).TrimStart('_'), _mobileAppImageWarningFileSizeInKB)
                .As<IImageService>();
        }
    }
}
