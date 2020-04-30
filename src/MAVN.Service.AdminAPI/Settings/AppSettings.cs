using JetBrains.Annotations;
using Lykke.Job.TokensStatistics.Client;
using MAVN.Service.AdminManagement.Client;
using Lykke.Service.AgentManagement.Client;
using Lykke.Service.BonusCustomerProfile.Client;
using Lykke.Service.Campaign.Client;
using Lykke.Service.Credentials.Client;
using Lykke.Service.CrossChainTransfers.Client;
using Lykke.Service.CrossChainWalletLinker.Client;
using Lykke.Service.CurrencyConvertor.Client;
using Lykke.Service.CustomerManagement.Client;
using MAVN.Service.CustomerProfile.Client;
using Lykke.Service.DashboardStatistics.Client;
using Lykke.Service.OperationsHistory.Client;
using MAVN.Service.PartnerManagement.Client;
using Lykke.Service.PaymentTransfers.Client;
using Lykke.Service.PrivateBlockchainFacade.Client;
using Lykke.Service.QuorumExplorer.Client;
using Lykke.Service.Referral.Client;
using Lykke.Service.Reporting.Client;
using Lykke.Service.Sessions.Client;
using Lykke.Service.Tiers.Client;
using Lykke.Service.Vouchers.Client;
using Lykke.Service.WalletManagement.Client;
using MAVN.Service.AdminAPI.Settings.Clients;
using MAVN.Service.AdminAPI.Settings.Service;
using MAVN.Service.AdminAPI.Settings.Slack;
using MAVN.Service.PaymentManagement.Client;
using MAVN.Service.SmartVouchers.Client;

namespace MAVN.Service.AdminAPI.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AppSettings
    {
        public AdminApiSettings AdminApiService { get; set; }

        public CampaignServiceClientSettings CampaignServiceClient { get; set; }

        public BonusCustomerProfileServiceClientSettings BonusCustomerProfileServiceClient { get; set; }

        public CustomerManagementServiceClientSettings CustomerManagementServiceClient { get; set; }

        public ReferralServiceClientSettings ReferralServiceClient { get; set; }

        public SessionsServiceClientSettings SessionsServiceClient { get; set; }

        public AdminManagementServiceClientSettings AdminManagementServiceClient { get; set; }

        public CustomerProfileServiceClientSettings CustomerProfileServiceClient { get; set; }

        public OperationsHistoryServiceClientSettings OperationsHistoryServiceClient { get; set; }

        public TokensStatisticsJobClientSettings TokensStatisticsJobClient { get; set; }

        public SlackNotificationsSettings SlackNotifications { get; set; }

        public MonitoringServiceClientSettings MonitoringServiceClient { get; set; }

        public PrivateBlockchainFacadeServiceClientSettings PrivateBlockchainFacadeClient { get; set; }

        public PaymentTransfersServiceClientSettings PaymentTransfersServiceClient { get; set; }

        public DashboardStatisticsServiceClientSettings DashboardStatisticsServiceClient { get; set; }

        public QuorumExplorerServiceClientSettings QuorumExplorerServiceClient { get; set; }

        public WalletManagementServiceClientSettings  WalletManagementServiceClient { get; set; }

        public PartnerManagementServiceClientSettings PartnerManagementServiceClient { get; set; }

        public TiersServiceClientSettings TiersServiceClient { get; set; }
        
        public AgentManagementServiceClientSettings AgentManagementServiceClient { get; set; }

        public CurrencyConvertorServiceClientSettings CurrencyConverterServiceClient { get; set; }
        
        public CrossChainWalletLinkerServiceClientSettings CrossChainWalletLinkerServiceClient { get; set; }

        public CrossChainTransfersServiceClientSettings CrossChainTransfersServiceClient { get; set; }

        public CredentialsServiceClientSettings CredentialsServiceClient { get; set; }

        public ReportServiceClientSettings ReportServiceClient { get; set; }

        public VouchersServiceClientSettings VouchersServiceClient { get; set; }

        public SmartVouchersServiceClientSettings SmartVouchersServiceClient { get; set; }

        public PaymentManagementServiceClientSettings PaymentManagementServiceClient { get; set; }
    }
}
