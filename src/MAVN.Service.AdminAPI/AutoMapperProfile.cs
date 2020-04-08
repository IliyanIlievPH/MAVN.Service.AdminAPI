using System;
using System.Linq;
using AutoMapper;
using JetBrains.Annotations;
using Lykke.Service.AgentManagement.Client.Models.Requirements;
using Lykke.Service.Campaign.Client.Models.BurnRule.Requests;
using Lykke.Service.Campaign.Client.Models.BurnRule.Responses;
using Lykke.Service.Campaign.Client.Models.Files.Requests;
using Lykke.Service.CrossChainWalletLinker.Client.Models;
using Lykke.Service.PartnerManagement.Client.Models.Location;
using Lykke.Service.PartnerManagement.Client.Models.Partner;
using Lykke.Service.PaymentTransfers.Client.Models.Requests;
using Lykke.Service.QuorumExplorer.Client.Models;
using Lykke.Service.Referral.Client.Models.Responses;
using Lykke.Service.Reporting.Client.Models;
using MAVN.Service.AdminAPI.Domain.Enums;
using MAVN.Service.AdminAPI.Domain.Models;
using MAVN.Service.AdminAPI.Models.ActionRules;
using MAVN.Service.AdminAPI.Models.Admins;
using MAVN.Service.AdminAPI.Models.Auth;
using MAVN.Service.AdminAPI.Models.Blockchain;
using MAVN.Service.AdminAPI.Models.BonusTypes;
using MAVN.Service.AdminAPI.Models.BurnRules;
using MAVN.Service.AdminAPI.Models.Customers;
using MAVN.Service.AdminAPI.Models.Dashboard;
using MAVN.Service.AdminAPI.Models.EarnRules;
using MAVN.Service.AdminAPI.Models.Locations.Requests;
using MAVN.Service.AdminAPI.Models.Locations.Responses;
using MAVN.Service.AdminAPI.Models.Partners.Requests;
using MAVN.Service.AdminAPI.Models.Partners.Responses;
using MAVN.Service.AdminAPI.Models.Payments;
using MAVN.Service.AdminAPI.Models.Reports;
using MAVN.Service.AdminAPI.Models.Settings;
using MAVN.Service.AdminAPI.Models.Statistics;
using MAVN.Service.AdminAPI.Models.Tiers;
using BurnRuleCreateRequest = MAVN.Service.AdminAPI.Models.BurnRules.BurnRuleCreateRequest;
using CustomerActivityStatus = Lykke.Service.CustomerManagement.Client.Enums.CustomerActivityStatus;
using CustomersStatisticResponseModel = MAVN.Service.AdminAPI.Models.Dashboard.CustomersStatisticResponse;
using CustomerStatisticsByDayResponseModel = MAVN.Service.AdminAPI.Models.Dashboard.CustomerStatisticsByDayResponse;
using CustomerWalletActivityStatus = Lykke.Service.WalletManagement.Client.Enums.CustomerWalletActivityStatus;
using PublicAddressStatus = MAVN.Service.AdminAPI.Models.Customers.Enums.PublicAddressStatus;

namespace MAVN.Service.AdminAPI
{
    [UsedImplicitly]
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Admin
            CreateMap<Lykke.Service.AdminManagement.Client.Models.AdminUser, Admin>()
                .ForMember(x => x.Id, x => x.MapFrom(y => y.AdminUserId))
                .ForMember(x => x.Registered, x => x.MapFrom(y => y.RegisteredAt));
            CreateMap<Admin, Lykke.Service.AdminManagement.Client.Models.AdminUser>()
                .ForMember(x => x.AdminUserId, x => x.MapFrom(y => y.Id))
                .ForMember(x => x.RegisteredAt, x => x.MapFrom(y => y.Registered));

            CreateMap<Admin, AdminModel>();
            CreateMap<AdminModel, Admin>();
            
            CreateMap<Lykke.Service.AdminManagement.Client.Models.AdminPermission, Permission>();
            CreateMap<Permission, Lykke.Service.AdminManagement.Client.Models.AdminPermission>();
            
            CreateMap<AdminPermission, Permission>();
            CreateMap<Permission, AdminPermission>();
            
            CreateMap<PermissionType, AdminPermissionType>();
            CreateMap<AdminPermissionType, PermissionType>();
            
            CreateMap<Lykke.Service.AdminManagement.Client.Models.AdminPermission, AdminPermission>();
            CreateMap<AdminPermission, Lykke.Service.AdminManagement.Client.Models.AdminPermission>();

            CreateMap<AdminPermissionLevel, PermissionLevel>();
            CreateMap<PermissionLevel, AdminPermissionLevel>();
            
            // Auth
            CreateMap<LoginModel,
                Lykke.Service.AdminManagement.Client.Models.AuthenticateRequestModel>(MemberList.Destination);

            // Bonus Types
            CreateMap<Lykke.Service.Campaign.Client.Models.BonusType.BonusTypeModel,
                BonusTypeModel>(MemberList.Destination);

            // Earn rules
            CreateMap<EarnRuleCreateModel,
                    Lykke.Service.Campaign.Client.Models.Campaign.Requests.CampaignCreateModel>(MemberList
                    .Destination)
                .ForMember(dest => dest.Contents, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.ToDate,
                    opt => opt.MapFrom(src => src.ToDate.HasValue ? src.ToDate.Value.Date.AddDays(1).AddMilliseconds(-1) : src.ToDate));

            CreateMap<Lykke.Service.Campaign.Client.Models.Campaign.Responses.CampaignDetailResponseModel,
                    EarnRuleModel>(MemberList.Destination)
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.CampaignStatus))
                .ForMember(dest => dest.Asset, opt => opt.Ignore())
                .ForMember(dest => dest.MobileContents, opt => opt.Ignore());

            CreateMap<EarnRuleUpdateModel,
                Lykke.Service.Campaign.Client.Models.Campaign.Requests.CampaignEditModel>(MemberList.Destination)
                .ForMember(dest => dest.Contents, opt => opt.Ignore())
                .ForMember(dest => dest.ToDate,
                    opt => opt.MapFrom(src => src.ToDate.HasValue ? src.ToDate.Value.Date.AddDays(1).AddMilliseconds(-1) : src.ToDate));

            CreateMap<ConditionBaseModel, Lykke.Service.Campaign.Client.Models.Condition.ConditionBaseModel>(MemberList.Destination)
                .ForMember(dest => dest.PartnerIds,
                    opt => opt.MapFrom(src => src.PartnerId.HasValue ? new[] {src.PartnerId.Value} : null))
                .IncludeAllDerived();

            CreateMap<ConditionCreateModel,
                Lykke.Service.Campaign.Client.Models.Condition.ConditionCreateModel>(MemberList.Destination);

            CreateMap<Lykke.Service.Campaign.Client.Models.Condition.ConditionModel,
                    ConditionModel>(MemberList.Destination)
                .ForMember(c => c.DisplayName, opt => opt.MapFrom(src => src.TypeDisplayName))
                .ForMember(dest => dest.PartnerId,
                    opt => opt.MapFrom(src =>
                        src.PartnerIds != null && src.PartnerIds.Length > 0 ? (Guid?) src.PartnerIds.First() : null));

            CreateMap<ConditionUpdateModel,
                Lykke.Service.Campaign.Client.Models.Condition.ConditionEditModel>(MemberList.Destination);

            CreateMap<Lykke.Service.Campaign.Client.Models.Campaign.Responses.CampaignResponse,
                    EarnRuleRowModel>(MemberList.Destination)
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.CampaignStatus))
                .ForMember(dest => dest.Asset, opt => opt.Ignore())
                .ForMember(dest => dest.Vertical, opt => opt.Ignore());

            // Burn Rules

            CreateMap<BurnRuleListRequest, BurnRulePaginationRequest>(MemberList.Destination)
                .ForMember(dest => dest.PartnerId, opt => opt.Ignore());

            CreateMap<BurnRuleCreateRequest, Lykke.Service.Campaign.Client.Models.BurnRule.Requests.BurnRuleCreateRequest>(
                    MemberList.Destination)
                .ForMember(dest => dest.Description,
                    opt => opt.MapFrom(src => string.IsNullOrEmpty(src.Description) ? null : src.Description))
                .ForMember(dest => dest.Vertical, opt => opt.MapFrom(src => src.BusinessVertical))
                .ForMember(dest => dest.BurnRuleContents, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore());

            CreateMap<BurnRuleInfoResponse, BurnRuleInfoModel>(MemberList.Destination)
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title));

            CreateMap<BurnRuleResponse, BurnRuleModel>(MemberList.Destination)
                .ForMember(dest => dest.BusinessVertical, opt => opt.MapFrom(src => src.Vertical))
                .ForMember(dest => dest.MobileContents, opt => opt.Ignore())
                .ForMember(dest => dest.VouchersCount, opt => opt.Ignore())
                .ForMember(dest => dest.VouchersInStockCount, opt => opt.Ignore());

            CreateMap<BurnRuleUpdateRequest, BurnRuleEditRequest>(MemberList.Destination)
                .ForMember(dest => dest.Description,
                    opt => opt.MapFrom(src => string.IsNullOrEmpty(src.Description) ? null : src.Description))
                .ForMember(dest => dest.Vertical, opt => opt.MapFrom(src => src.BusinessVertical))
                .ForMember(dest => dest.BurnRuleContents, opt => opt.Ignore());

            CreateMap<ImageAddRequest, FileCreateRequest>(MemberList.Destination)
                .ForMember(dest => dest.Type, opt => opt.Ignore())
                .ForMember(dest => dest.Name, opt => opt.Ignore())
                .ForMember(dest => dest.Content, opt => opt.Ignore());

            CreateMap<ImageEditRequest, FileEditRequest>(MemberList.Destination)
                .ForMember(dest => dest.Type, opt => opt.Ignore())
                .ForMember(dest => dest.Name, opt => opt.Ignore())
                .ForMember(dest => dest.Content, opt => opt.Ignore());

            // Customers
            CreateMap<Lykke.Service.CustomerProfile.Client.Models.Responses.CustomerProfile, CustomerModel>(MemberList.Destination)
                .ForMember(c => c.RegisteredDate, opt => opt.MapFrom(src => src.Registered))
                .ForMember(dest => dest.ReferralCode, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerStatus, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerAgentStatus, opt => opt.Ignore());

            CreateMap<Lykke.Service.CustomerProfile.Client.Models.Responses.CustomerProfile, CustomerDetailsModel>(MemberList.Destination)
                .ForMember(c => c.RegisteredDate, opt => opt.MapFrom(src => src.Registered))
                .ForMember(dest => dest.ReferralCode, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerStatus, opt => opt.Ignore())
                .ForMember(dest => dest.AgentStatus, opt => opt.Ignore())
                .ForMember(dest => dest.WalletStatus, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerAgentStatus, opt => opt.Ignore());
            
            // Payments
            CreateMap<PaymentListRequest, PaginatedRequest>(MemberList.Destination);

            CreateMap<CustomerActivityStatus, Models.Customers.Enums.CustomerActivityStatus>();
            CreateMap<CustomerWalletActivityStatus, Models.Customers.Enums.CustomerWalletActivityStatus>();

            // Wallets
            CreateMap<Lykke.Service.PrivateBlockchainFacade.Client.Models.CustomerWalletAddressResponseModel, CustomerPrivateWalletAddressResponse>();

            CreateMap<PublicAddressResponseModel, CustomerPublicWalletAddressResponse>();
            CreateMap<Lykke.Service.CrossChainWalletLinker.Client.Models.PublicAddressStatus, PublicAddressStatus>();

            //Leads
            CreateMap<LeadStatisticsResponse, LeadStatisticModel>();

            // Blockchain
            CreateMap<EventParameter, EventParameters>(MemberList.Destination);

            CreateMap<Event, EventModel>(MemberList.Destination)
                .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => ToDateTime(src.Timestamp)));

            CreateMap<Transaction, TransactionModel>(MemberList.Destination)
                .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => ToDateTime(src.Timestamp)))
                .ForMember(dest => dest.Events, opt => opt.Ignore());

            CreateMap<TransactionDetailedInfo, TransactionModel>(MemberList.Destination)
                .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => ToDateTime(src.Timestamp)))
                .ForMember(dest => dest.ContractAddress, opt => opt.Ignore())
                .ForMember(dest => dest.FunctionName, opt => opt.Ignore())
                .ForMember(dest => dest.FunctionSignature, opt => opt.Ignore());

            CreateMap<CustomerOperation, CustomerOperationModel>();

            //Dashboard
            CreateMap<Lykke.Service.DashboardStatistics.Client.Models.Leads.LeadsListResponseModel, LeadsListResponse>();
            CreateMap<Lykke.Service.DashboardStatistics.Client.Models.Leads.LeadsStatisticsForDayReportModel, LeadsStatisticsForDayReportModel>();
            CreateMap<Lykke.Service.DashboardStatistics.Client.Models.Leads.LeadsStatisticsModel, LeadsStatistics>();

            CreateMap<Lykke.Service.DashboardStatistics.Client.Models.Tokens.TokensListResponseModel, TokensListResponse>();
            CreateMap<Lykke.Service.DashboardStatistics.Client.Models.Tokens.TokensStatisticsModel, TokensStatistics>();

            CreateMap<Lykke.Service.DashboardStatistics.Client.Models.Customers.CustomersStatisticResponse, CustomersStatisticResponseModel>();
            CreateMap<Lykke.Service.DashboardStatistics.Client.Models.Customers.CustomerStatisticsByDayResponse, CustomerStatisticsByDayResponseModel>();

            CreateMap<TokensListRequest, Lykke.Service.DashboardStatistics.Client.Models.Tokens.TokensListRequestModel>();
            CreateMap<CustomersListRequest, Lykke.Service.DashboardStatistics.Client.Models.Customers.CustomersListRequestModel>();
            CreateMap<LeadsListRequest, Lykke.Service.DashboardStatistics.Client.Models.Leads.LeadsListRequestModel>();

            //Partners
            CreateMap<PartnerListRequest, PartnerListRequestModel>()
                .ForMember(dest => dest.Vertical, opt => opt.MapFrom(src => src.BusinessVertical));

            CreateMap<PartnerListDetailsModel, PartnerRowResponse>(MemberList.Destination);

            CreateMap<PartnerDetailsModel, PartnerDetailsResponse>()
                .ForMember(dest => dest.ClientSecret, opt => opt.Ignore());

            CreateMap<PartnerCreateRequest, PartnerCreateModel>()
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore());

            CreateMap<PartnerUpdateRequest, PartnerUpdateModel>();

            CreateMap<LocationCreateRequest, LocationCreateModel>()
                .ForMember(dest=>dest.ContactPerson, opt=>opt.MapFrom(src=>
                    new Lykke.Service.PartnerManagement.Client.Models.ContactPersonModel
                    {
                         Email = src.Email,
                         FirstName = src.FirstName,
                         LastName = src.LastName,
                         PhoneNumber = src.Phone
                    } ));

            CreateMap<LocationEditRequest, LocationUpdateModel>()
                .ForMember(dest => dest.ContactPerson, opt => opt.MapFrom(src =>
                    new Lykke.Service.PartnerManagement.Client.Models.ContactPersonModel
                    {
                        Email = src.Email,
                        FirstName = src.FirstName,
                        LastName = src.LastName,
                        PhoneNumber = src.Phone
                    }));

            CreateMap<LocationDetailsModel, LocationResponse>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.ContactPerson.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.ContactPerson.LastName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.ContactPerson.Email))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.ContactPerson.PhoneNumber));

            // Reports
            CreateMap<TransactionReport, ReportItemModel>(MemberList.Destination);
            
            // Tiers
            CreateMap<Lykke.Service.Tiers.Client.Models.Reports.TierCustomersCountModel,
                TierModel>(MemberList.Destination);

            //Settings
            CreateMap<TokensRequirementModel, AgentRequirementResponse>()
                .ForMember(dest => dest.TokensAmount, opt => opt.MapFrom(src => src.RequiredNumberOfTokens));
            CreateMap<AgentRequirementUpdateRequest, UpdateTokensRequirementModel>()
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.TokensAmount));

            CreateMap< Lykke.Service.CurrencyConvertor.Client.Models.Responses.GlobalCurrencyRateModel, GlobalCurrencyRateModel>();
            CreateMap<GlobalCurrencyRateModel, Lykke.Service.CurrencyConvertor.Client.Models.Requests.GlobalCurrencyRateRequest>();
        }

        private static DateTime ToDateTime(long input)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(input);
        }
    }
}
