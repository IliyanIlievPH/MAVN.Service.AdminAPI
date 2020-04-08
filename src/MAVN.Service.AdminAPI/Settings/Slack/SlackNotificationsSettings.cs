using JetBrains.Annotations;

namespace MAVN.Service.AdminAPI.Settings.Slack
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class SlackNotificationsSettings
    {
        public AzureQueueSettings AzureQueue { get; set; }
    }
}
