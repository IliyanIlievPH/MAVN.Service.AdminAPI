using JetBrains.Annotations;
using Lykke.SettingsReader.Attributes;
using MAVN.Service.AdminAPI.Settings.Service.Db;

namespace MAVN.Service.AdminAPI.Settings.Service
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AdminApiSettings
    {
        public string TokenSymbol { get; set; }

        public bool MobileAppImageDoOptimization { get; set; }

        public int MobileAppImageMinWidth { get; set; }

        public int MobileAppImageWarningFileSizeInKB { get; set; }

        public int SuggestedAdminPasswordLength { set; get; }

        public DbSettings Db { get; set; }

        [Optional]
        public bool IsPublicBlockchainFeatureDisabled { get; set; }

        [Optional]
        public bool IsPhoneVerificationDisabled { get; set; }

        public string ReferralUrlTemplate { get; set; }
    }
}
