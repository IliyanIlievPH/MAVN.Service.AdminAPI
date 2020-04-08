using JetBrains.Annotations;

namespace MAVN.Service.AdminAPI.Settings.Clients
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class MonitoringServiceClientSettings
    {
        public string MonitoringServiceUrl { get; set; }
    }
}
