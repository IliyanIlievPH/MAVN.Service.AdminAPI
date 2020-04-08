using Autofac;
using Falcon.Common.Middleware.Authentication;
using JetBrains.Annotations;
using Lykke.Job.TokensStatistics.Client;
using Lykke.Service.AdminManagement.Client;
using Lykke.Service.AgentManagement.Client;
using Lykke.Service.BonusCustomerProfile.Client;
using Lykke.Service.Campaign.Client;
using Lykke.Service.Credentials.Client;
using Lykke.Service.CrossChainTransfers.Client;
using Lykke.Service.CrossChainWalletLinker.Client;
using Lykke.Service.CurrencyConvertor.Client;
using Lykke.Service.CustomerManagement.Client;
using Lykke.Service.CustomerProfile.Client;
using Lykke.Service.DashboardStatistics.Client;
using Lykke.Service.OperationsHistory.Client;
using Lykke.Service.PartnerManagement.Client;
using Lykke.Service.PaymentTransfers.Client;
using Lykke.Service.PrivateBlockchainFacade.Client;
using Lykke.Service.QuorumExplorer.Client;
using Lykke.Service.Referral.Client;
using Lykke.Service.Reporting.Client;
using Lykke.Service.Sessions.Client;
using Lykke.Service.Tiers.Client;
using Lykke.Service.Vouchers.Client.Extensions;
using Lykke.Service.WalletManagement.Client;
using Lykke.SettingsReader;
using MAVN.Service.AdminAPI.Domain.Services;
using MAVN.Service.AdminAPI.DomainServices;
using MAVN.Service.AdminAPI.Settings;

namespace MAVN.Service.AdminAPI
{
    [UsedImplicitly]
    public class AutofacModule : Module
    {
        private readonly AppSettings _appSettings;

        public AutofacModule(IReloadingManager<AppSettings> appSettings)
        {
            _appSettings = appSettings.CurrentValue;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<RequestContext>()
                .As<IRequestContext>()
                .InstancePerLifetimeScope();

            builder.RegisterType<ReferralService>()
                .As<IReferralService>()
                .SingleInstance();

            builder.RegisterType<HistoryConverter>()
                .As<IHistoryConverter>()
                .SingleInstance()
                .WithParameter(TypedParameter.From(_appSettings.AdminApiService.TokenSymbol));

            builder.RegisterType<AdminsService>()
                .As<IAdminsService>()
                .SingleInstance();

            builder.RegisterAgentManagementClient(_appSettings.AgentManagementServiceClient);
            builder.RegisterCredentialsClient(_appSettings.CredentialsServiceClient);
            builder.RegisterCampaignClient(_appSettings.CampaignServiceClient);
            builder.RegisterBonusCustomerProfileClient(_appSettings.BonusCustomerProfileServiceClient, null);
            builder.RegisterCustomerManagementClient(_appSettings.CustomerManagementServiceClient, null);
            builder.RegisterReferralClient(_appSettings.ReferralServiceClient, null);
            builder.RegisterSessionsServiceClient(_appSettings.SessionsServiceClient);
            builder.RegisterAdminManagementClient(_appSettings.AdminManagementServiceClient);
            builder.RegisterCustomerProfileClient(_appSettings.CustomerProfileServiceClient);
            builder.RegisterOperationsHistoryClient(_appSettings.OperationsHistoryServiceClient, null);
            builder.RegisterTokensStatisticsClient(_appSettings.TokensStatisticsJobClient, null);
            builder.RegisterPrivateBlockchainFacadeClient(_appSettings.PrivateBlockchainFacadeClient, null);
            builder.RegisterPaymentTransfersClient(_appSettings.PaymentTransfersServiceClient, null);
            builder.RegisterQuorumExplorerClient(_appSettings.QuorumExplorerServiceClient, null);
            builder.RegisterWalletManagementClient(_appSettings.WalletManagementServiceClient, null);
            builder.RegisterDashboardStatisticsClient(_appSettings.DashboardStatisticsServiceClient, null);
            builder.RegisterTiersClient(_appSettings.TiersServiceClient);
            builder.RegisterPartnerManagementClient(_appSettings.PartnerManagementServiceClient);
            builder.RegisterCurrencyConvertorClient(_appSettings.CurrencyConverterServiceClient);
            builder.RegisterCrossChainWalletLinkerClient(_appSettings.CrossChainWalletLinkerServiceClient, null);
            builder.RegisterCrossChainTransfersClient(_appSettings.CrossChainTransfersServiceClient, null);
            builder.RegisterReportClient(_appSettings.ReportServiceClient, null);
            builder.RegisterVouchersClient(_appSettings.VouchersServiceClient);
        }
    }
}
