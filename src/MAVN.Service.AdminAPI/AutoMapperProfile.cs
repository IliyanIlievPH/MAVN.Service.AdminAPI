﻿using System;
using System.Linq;
using AutoMapper;
using JetBrains.Annotations;
using MAVN.Service.Campaign.Client.Models.BurnRule.Requests;
using MAVN.Service.Campaign.Client.Models.BurnRule.Responses;
using MAVN.Service.Campaign.Client.Models.Files.Requests;
using MAVN.Service.CrossChainWalletLinker.Client.Models;
using MAVN.Service.PartnerManagement.Client.Models.Location;
using MAVN.Service.PartnerManagement.Client.Models.Partner;
using MAVN.Service.QuorumExplorer.Client.Models;
using MAVN.Service.Referral.Client.Models.Responses;
using MAVN.Service.Reporting.Client.Models;
using MAVN.Service.AdminAPI.Domain.Enums;
using MAVN.Service.AdminAPI.Domain.Models;
using MAVN.Service.AdminAPI.Models.ActionRules;
using MAVN.Service.AdminAPI.Models.Auth;
using MAVN.Service.AdminAPI.Models.Blockchain;
using MAVN.Service.AdminAPI.Models.BonusTypes;
using MAVN.Service.AdminAPI.Models.BurnRules;
using MAVN.Service.AdminAPI.Models.Common;
using MAVN.Service.AdminAPI.Models.Customers;
using MAVN.Service.AdminAPI.Models.Dashboard;
using MAVN.Service.AdminAPI.Models.EarnRules;
using MAVN.Service.AdminAPI.Models.Kyc.Requests;
using MAVN.Service.AdminAPI.Models.Kyc.Responses;
using MAVN.Service.AdminAPI.Models.Locations.Requests;
using MAVN.Service.AdminAPI.Models.Locations.Responses;
using MAVN.Service.AdminAPI.Models.Partners.Requests;
using MAVN.Service.AdminAPI.Models.Partners.Responses;
using MAVN.Service.AdminAPI.Models.PaymentProviderDetails;
using MAVN.Service.AdminAPI.Models.Reports;
using MAVN.Service.AdminAPI.Models.Settings;
using MAVN.Service.AdminAPI.Models.SmartVouchers.Campaigns;
using MAVN.Service.AdminAPI.Models.SmartVouchers.Vouchers;
using MAVN.Service.AdminAPI.Models.Statistics;
using MAVN.Service.AdminAPI.Models.Tiers;
using MAVN.Service.SmartVouchers.Client.Models.Requests;
using MAVN.Service.SmartVouchers.Client.Models.Responses;
using AvailablePaymentProvidersRequirementsResponse = MAVN.Service.AdminAPI.Models.PaymentProviderDetails.AvailablePaymentProvidersRequirementsResponse;
using BurnRuleCreateRequest = MAVN.Service.AdminAPI.Models.BurnRules.BurnRuleCreateRequest;
using CustomerActivityStatus = MAVN.Service.CustomerManagement.Client.Enums.CustomerActivityStatus;
using CustomersStatisticResponseModel = MAVN.Service.AdminAPI.Models.Dashboard.CustomersStatisticResponse;
using CustomerStatisticsByDayResponseModel = MAVN.Service.AdminAPI.Models.Dashboard.CustomerStatisticsByDayResponse;
using CustomerWalletActivityStatus = MAVN.Service.WalletManagement.Client.Enums.CustomerWalletActivityStatus;
using KycInformationResponse = MAVN.Service.AdminAPI.Models.Kyc.Responses.KycInformationResponse;
using KycStatusChangeResponse = MAVN.Service.AdminAPI.Models.Kyc.Responses.KycStatusChangeResponse;
using PaymentProviderProperties = MAVN.Service.AdminAPI.Models.PaymentProviderDetails.PaymentProviderProperties;
using PaymentProviderProperty = MAVN.Service.AdminAPI.Models.PaymentProviderDetails.PaymentProviderProperty;
using PublicAddressStatus = MAVN.Service.AdminAPI.Models.Customers.Enums.PublicAddressStatus;

namespace MAVN.Service.AdminAPI
{
    [UsedImplicitly]
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            #region Admin

            CreateMap<MAVN.Service.AdminManagement.Client.Models.AdminUser, AdminModel>()
                .ForMember(x => x.Id, x => x.MapFrom(y => y.AdminUserId))
                .ForMember(x => x.Registered, x => x.MapFrom(y => y.RegisteredAt));
            CreateMap<AdminModel, MAVN.Service.AdminManagement.Client.Models.AdminUser>()
                .ForMember(x => x.AdminUserId, x => x.MapFrom(y => y.Id))
                .ForMember(x => x.RegisteredAt, x => x.MapFrom(y => y.Registered));

            CreateMap<AdminLocalization, MAVN.Service.AdminManagement.Client.Models.Enums.Localization>();

            CreateMap<MAVN.Service.AdminManagement.Client.Models.AdminPermission, Permission>();
            CreateMap<Permission, MAVN.Service.AdminManagement.Client.Models.AdminPermission>();

            // Auth
            CreateMap<LoginModel,
                MAVN.Service.AdminManagement.Client.Models.AuthenticateRequestModel>(MemberList.Destination);

            #endregion

            // Bonus Types
            CreateMap<MAVN.Service.Campaign.Client.Models.BonusType.BonusTypeModel,
                BonusTypeModel>(MemberList.Destination);

            #region Earn rules

            CreateMap<EarnRuleCreateModel,
                    MAVN.Service.Campaign.Client.Models.Campaign.Requests.CampaignCreateModel>(MemberList
                    .Destination)
                .ForMember(dest => dest.Contents, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.ToDate,
                    opt => opt.MapFrom(src => src.ToDate.HasValue ? src.ToDate.Value.Date.AddDays(1).AddMilliseconds(-1) : src.ToDate));

            CreateMap<MAVN.Service.Campaign.Client.Models.Campaign.Responses.CampaignDetailResponseModel,
                    EarnRuleModel>(MemberList.Destination)
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.CampaignStatus))
                .ForMember(dest => dest.Asset, opt => opt.Ignore())
                .ForMember(dest => dest.MobileContents, opt => opt.Ignore());

            CreateMap<EarnRuleUpdateModel,
                    MAVN.Service.Campaign.Client.Models.Campaign.Requests.CampaignEditModel>(MemberList.Destination)
                .ForMember(dest => dest.Contents, opt => opt.Ignore())
                .ForMember(dest => dest.ToDate,
                    opt => opt.MapFrom(src => src.ToDate.HasValue ? src.ToDate.Value.Date.AddDays(1).AddMilliseconds(-1) : src.ToDate));

            CreateMap<ConditionBaseModel, MAVN.Service.Campaign.Client.Models.Condition.ConditionBaseModel>(MemberList.Destination)
                .ForMember(dest => dest.PartnerIds,
                    opt => opt.MapFrom(src => src.PartnerId.HasValue ? new[] { src.PartnerId.Value } : null))
                .IncludeAllDerived();

            CreateMap<ConditionCreateModel,
                MAVN.Service.Campaign.Client.Models.Condition.ConditionCreateModel>(MemberList.Destination);

            CreateMap<MAVN.Service.Campaign.Client.Models.Condition.ConditionModel,
                    ConditionModel>(MemberList.Destination)
                .ForMember(c => c.DisplayName, opt => opt.MapFrom(src => src.TypeDisplayName))
                .ForMember(dest => dest.PartnerId,
                    opt => opt.MapFrom(src =>
                        src.PartnerIds != null && src.PartnerIds.Length > 0 ? (Guid?)src.PartnerIds.First() : null));

            CreateMap<ConditionUpdateModel,
                MAVN.Service.Campaign.Client.Models.Condition.ConditionEditModel>(MemberList.Destination);

            CreateMap<MAVN.Service.Campaign.Client.Models.Campaign.Responses.CampaignResponse,
                    EarnRuleRowModel>(MemberList.Destination)
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.CampaignStatus))
                .ForMember(dest => dest.Asset, opt => opt.Ignore())
                .ForMember(dest => dest.Vertical, opt => opt.Ignore());

            #endregion

            #region Burn Rules

            CreateMap<BurnRuleListRequest, BurnRulePaginationRequest>(MemberList.Destination)
                .ForMember(dest => dest.PartnerId, opt => opt.Ignore());

            CreateMap<BurnRuleCreateRequest, MAVN.Service.Campaign.Client.Models.BurnRule.Requests.BurnRuleCreateRequest>(
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

            #endregion

            #region Rule Image

            CreateMap<ImageAddRequest, FileCreateRequest>(MemberList.Destination)
                .ForMember(dest => dest.Type, opt => opt.Ignore())
                .ForMember(dest => dest.Name, opt => opt.Ignore())
                .ForMember(dest => dest.Content, opt => opt.Ignore());

            CreateMap<ImageEditRequest, FileEditRequest>(MemberList.Destination)
                .ForMember(dest => dest.Type, opt => opt.Ignore())
                .ForMember(dest => dest.Name, opt => opt.Ignore())
                .ForMember(dest => dest.Content, opt => opt.Ignore());

            #endregion

            // Customers

            CreateMap<MAVN.Service.CustomerProfile.Client.Models.Responses.CustomerProfile, CustomerModel>(MemberList
                    .Destination)
                .ForMember(c => c.RegisteredDate, opt => opt.MapFrom(src => src.Registered))
                .ForMember(dest => dest.ReferralCode, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerStatus, opt => opt.Ignore());

            CreateMap<MAVN
                    .Service.CustomerProfile.Client.Models.Responses.CustomerProfile, CustomerDetailsModel>(
                    MemberList.Destination)
                .ForMember(c => c.RegisteredDate, opt => opt.MapFrom(src => src.Registered))
                .ForMember(dest => dest.ReferralCode, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerStatus, opt => opt.Ignore())
                .ForMember(dest => dest.WalletStatus, opt => opt.Ignore());

            CreateMap<CustomerActivityStatus, Models.Customers.Enums.CustomerActivityStatus>();
            CreateMap<CustomerWalletActivityStatus, Models.Customers.Enums.CustomerWalletActivityStatus>();

            // Wallets
            CreateMap<MAVN.Service.PrivateBlockchainFacade.Client.Models.CustomerWalletAddressResponseModel, CustomerPrivateWalletAddressResponse>();

            CreateMap<PublicAddressResponseModel, CustomerPublicWalletAddressResponse>();
            CreateMap<MAVN.Service.CrossChainWalletLinker.Client.Models.PublicAddressStatus, PublicAddressStatus>();

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
            CreateMap<MAVN.Service.DashboardStatistics.Client.Models.Tokens.TokensListResponseModel, TokensListResponse>();
            CreateMap<MAVN.Service.DashboardStatistics.Client.Models.Tokens.TokensStatisticsModel, TokensStatistics>();

            CreateMap<MAVN.Service.DashboardStatistics.Client.Models.Customers.CustomersStatisticResponse, CustomersStatisticResponseModel>();
            CreateMap<MAVN.Service.DashboardStatistics.Client.Models.Customers.CustomerStatisticsByDayResponse, CustomerStatisticsByDayResponseModel>();

            CreateMap<TokensListRequest, MAVN.Service.DashboardStatistics.Client.Models.Tokens.TokensListRequestModel>();
            CreateMap<DashboardStatistics.Client.Models.VoucherStatistic.VoucherStatisticsResponse, VoucherStatisticsResponse>();
            CreateMap<DashboardStatistics.Client.Models.VoucherStatistic.VoucherDailyStatisticsModel, VoucherDailyStatisticsModel>();
            CreateMap<DashboardStatistics.Client.Models.VoucherStatistic.VoucherDailyStatisticsResponse, VoucherDailyStatisticsResponse>();

            //Partners
            CreateMap<PartnerListRequest, PartnerListRequestModel>()
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.Vertical, opt => opt.MapFrom(src => src.BusinessVertical));

            CreateMap<PartnerListDetailsModel, PartnerRowResponse>(MemberList.Destination)
                .ForMember(dest => dest.KycStatus, opt => opt.Ignore());

            CreateMap<PartnerDetailsModel, PartnerDetailsResponse>();

            CreateMap<PartnerCreateRequest, PartnerCreateModel>()
                .ForMember(dest => dest.ClientId, opt => opt.Ignore())
                .ForMember(dest => dest.ClientSecret, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore());

            CreateMap<PartnerUpdateRequest, PartnerUpdateModel>()
                .ForMember(dest => dest.ClientId, opt => opt.Ignore())
                .ForMember(dest => dest.ClientSecret, opt => opt.Ignore());
            CreateMap<PartnerManagement.Client.Models.PartnerLinking.PartnerLinkingInfoResponse,
                PartnerLinkingInfoResponse>();

            CreateMap<LocationCreateRequest, LocationCreateModel>()
                .ForMember(dest => dest.ContactPerson,
                    opt => opt.MapFrom(src =>
                        src.Email != null
                            ? new PartnerManagement.Client.Models.ContactPersonModel
                            {
                                Email = src.Email,
                                FirstName = src.FirstName,
                                LastName = src.LastName,
                                PhoneNumber = src.Phone
                            }
                            : null));

            CreateMap<LocationEditRequest, LocationUpdateModel>()
                .ForMember(dest => dest.ContactPerson,
                    opt => opt.MapFrom(src =>
                        src.Email != null
                            ? new PartnerManagement.Client.Models.ContactPersonModel
                            {
                                Email = src.Email,
                                FirstName = src.FirstName,
                                LastName = src.LastName,
                                PhoneNumber = src.Phone
                            }
                            : null));

            CreateMap<LocationDetailsModel, LocationResponse>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.ContactPerson.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.ContactPerson.LastName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.ContactPerson.Email))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.ContactPerson.PhoneNumber))
                .ForMember(dest => dest.CountryIso3Code, opt => opt.MapFrom(src => src.CountryIso3Code));

            CreateMap<CheckPartnerAbilityRequest, CheckAbilityRequest>();
            CreateMap<CheckAbilityResponse, CheckPartnerAbilityResponse>();

            // Reports
            CreateMap<TransactionReport, ReportItemModel>(MemberList.Destination);

            // Tiers
            CreateMap<MAVN.Service.Tiers.Client.Models.Reports.TierCustomersCountModel,
                TierModel>(MemberList.Destination);

            CreateMap<MAVN.Service.CurrencyConvertor.Client.Models.Responses.GlobalCurrencyRateModel, GlobalCurrencyRateModel>();
            CreateMap<GlobalCurrencyRateModel, MAVN.Service.CurrencyConvertor.Client.Models.Requests.GlobalCurrencyRateRequest>();

            #region Voucher Campaign

            CreateMap<VoucherCampaignResponseModel, SmartVoucherCampaignResponse>();
            CreateMap<VoucherCampaignDetailsResponseModel, SmartVoucherCampaignDetailsResponse>(MemberList.Destination)
                .ForMember(dest => dest.MobileContents, opt => opt.Ignore());

            CreateMap<SmartVoucherCampaignCreateRequest, VoucherCampaignCreateModel>(MemberList.Destination)
                .ForMember(dest => dest.Description,
                    opt => opt.MapFrom(src => string.IsNullOrEmpty(src.Description) ? null : src.Description))
                .ForMember(dest => dest.ToDate,
                    opt => opt.MapFrom(src => src.ToDate.HasValue ? src.ToDate.Value.Date.AddDays(1).AddMilliseconds(-1) : src.ToDate))
                .ForMember(dest => dest.LocalizedContents, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore());

            CreateMap<SmartVoucherCampaignEditRequest, VoucherCampaignEditModel>(MemberList.Destination)
                .ForMember(dest => dest.Description,
                    opt => opt.MapFrom(src => string.IsNullOrEmpty(src.Description) ? null : src.Description))
                .ForMember(dest => dest.ToDate,
                    opt => opt.MapFrom(src => src.ToDate.HasValue ? src.ToDate.Value.Date.AddDays(1).AddMilliseconds(-1) : src.ToDate))
                .ForMember(dest => dest.LocalizedContents, opt => opt.Ignore());

            CreateMap<SmartVoucherCampaignSetImageRequest, CampaignImageFileRequest>(MemberList.Destination)
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ContentId))
                .ForMember(dest => dest.Type, opt => opt.Ignore())
                .ForMember(dest => dest.Name, opt => opt.Ignore())
                .ForMember(dest => dest.Content, opt => opt.Ignore());

            CreateMap<VoucherResponseModel, SmartVoucherResponse>();
            CreateMap<PagedRequestModel, BasePaginationRequestModel>();

            #endregion

            CreateMap<CustomerProfile.Client.Models.Responses.PaymentProviderDetails, PaymentProviderDetails>();
            CreateMap<PaymentManagement.Client.Models.Responses.AvailablePaymentProvidersRequirementsResponse, AvailablePaymentProvidersRequirementsResponse>();
            CreateMap<PaymentManagement.Client.Models.Responses.PaymentProviderProperties, PaymentProviderProperties>();
            CreateMap<PaymentManagement.Client.Models.Responses.PaymentProviderProperty, PaymentProviderProperty>();

            // KYC
            CreateMap<MAVN.Service.Kyc.Client.Models.Responses.KycInformationResponse, KycInformationResponse>();
            CreateMap<MAVN.Service.Kyc.Client.Models.Responses.KycStatusChangeResponse, KycStatusChangeResponse>();
            CreateMap<MAVN.Service.Kyc.Client.Models.Responses.KycUpdateResponse, KycInformationUpdateResponse>();
        }

        private static DateTime ToDateTime(long input)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(input);
        }
    }
}
