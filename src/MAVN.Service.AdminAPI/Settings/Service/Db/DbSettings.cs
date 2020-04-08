using JetBrains.Annotations;
using Lykke.SettingsReader.Attributes;

namespace MAVN.Service.AdminAPI.Settings.Service.Db
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class DbSettings
    {
        [AzureTableCheck]
        public string LogsConnString { get; set; }
    }
}
