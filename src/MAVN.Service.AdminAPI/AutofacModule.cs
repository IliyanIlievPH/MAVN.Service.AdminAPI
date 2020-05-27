using Autofac;
using MAVN.Common.Middleware.Authentication;
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
using MAVN.Service.Vouchers.Client.Extensions;
using MAVN.Service.WalletManagement.Client;
using Lykke.SettingsReader;
using MAVN.Service.AdminAPI.Domain.Services;
using MAVN.Service.AdminAPI.DomainServices;
using MAVN.Service.AdminAPI.Infrastructure;
using MAVN.Service.AdminAPI.Settings;
using MAVN.Service.Kyc.Client;
using MAVN.Service.PaymentManagement.Client;
using MAVN.Service.SmartVouchers.Client;
using RequestContext = MAVN.Service.AdminAPI.Infrastructure.RequestContext;

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
                .As<IExtRequestContext>()
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
            builder.RegisterSmartVouchersClient(_appSettings.SmartVouchersServiceClient, null);
            builder.RegisterPaymentManagementClient(_appSettings.PaymentManagementServiceClient, null);
            builder.RegisterKycClient(_appSettings.KycServiceClient, null);
        }
    }
}
