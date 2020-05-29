using JetBrains.Annotations;
using MAVN.Job.TokensStatistics.Client;
using MAVN.Service.AdminManagement.Client;
using MAVN.Service.BonusCustomerProfile.Client;
using MAVN.Service.Campaign.Client;
using MAVN.Service.Credentials.Client;
using MAVN.Service.CrossChainTransfers.Client;
using MAVN.Service.CrossChainWalletLinker.Client;
using MAVN.Service.CurrencyConvertor.Client;
using MAVN.Service.CustomerManagement.Client;
using MAVN.Service.CustomerProfile.Client;
using MAVN.Service.DashboardStatistics.Client;
using MAVN.Service.OperationsHistory.Client;
using MAVN.Service.PartnerManagement.Client;
using MAVN.Service.PrivateBlockchainFacade.Client;
using MAVN.Service.QuorumExplorer.Client;
using MAVN.Service.Referral.Client;
using MAVN.Service.Reporting.Client;
using MAVN.Service.Sessions.Client;
using MAVN.Service.Tiers.Client;
using MAVN.Service.Vouchers.Client;
using MAVN.Service.WalletManagement.Client;
using MAVN.Service.AdminAPI.Settings.Clients;
using MAVN.Service.AdminAPI.Settings.Service;
using MAVN.Service.AdminAPI.Settings.Slack;
using MAVN.Service.Kyc.Client;
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

        public DashboardStatisticsServiceClientSettings DashboardStatisticsServiceClient { get; set; }

        public QuorumExplorerServiceClientSettings QuorumExplorerServiceClient { get; set; }

        public WalletManagementServiceClientSettings  WalletManagementServiceClient { get; set; }

        public PartnerManagementServiceClientSettings PartnerManagementServiceClient { get; set; }

        public TiersServiceClientSettings TiersServiceClient { get; set; }

        public CurrencyConvertorServiceClientSettings CurrencyConverterServiceClient { get; set; }
        
        public CrossChainWalletLinkerServiceClientSettings CrossChainWalletLinkerServiceClient { get; set; }

        public CrossChainTransfersServiceClientSettings CrossChainTransfersServiceClient { get; set; }

        public CredentialsServiceClientSettings CredentialsServiceClient { get; set; }

        public ReportServiceClientSettings ReportServiceClient { get; set; }

        public VouchersServiceClientSettings VouchersServiceClient { get; set; }

        public SmartVouchersServiceClientSettings SmartVouchersServiceClient { get; set; }

        public PaymentManagementServiceClientSettings PaymentManagementServiceClient { get; set; }

        public KycServiceClientSettings KycServiceClient { get; set; }
    }
}
