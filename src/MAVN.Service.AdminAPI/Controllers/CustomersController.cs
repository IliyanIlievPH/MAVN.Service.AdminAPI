using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Common;
using MAVN.Common.Middleware.Authentication;
using Lykke.Common.ApiLibrary.Exceptions;
using MAVN.Service.Campaign.Client;
using MAVN.Service.CrossChainWalletLinker.Client;
using MAVN.Service.CrossChainWalletLinker.Client.Models;
using MAVN.Service.CustomerManagement.Client;
using MAVN.Service.CustomerManagement.Client.Enums;
using MAVN.Service.CustomerManagement.Client.Models.Requests;
using MAVN.Service.OperationsHistory.Client;
using MAVN.Service.PrivateBlockchainFacade.Client;
using MAVN.Service.WalletManagement.Client;
using MAVN.Service.WalletManagement.Client.Enums;
using MAVN.Service.WalletManagement.Client.Models.Requests;
using MAVN.Service.AdminAPI.Domain.Enums;
using MAVN.Service.AdminAPI.Domain.Services;
using MAVN.Service.AdminAPI.Infrastructure.Constants;
using MAVN.Service.AdminAPI.Infrastructure.CustomAttributes;
using MAVN.Service.AdminAPI.Models.Common;
using MAVN.Service.AdminAPI.Models.Customers;
using MAVN.Service.AdminAPI.Models.Customers.Enums;
using MAVN.Service.CustomerProfile.Client;
using MAVN.Service.CustomerProfile.Client.Models;
using MAVN.Service.CustomerProfile.Client.Models.Requests;
using MAVN.Service.CustomerProfile.Client.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using MoreLinq;
using CustomerActivityStatus = MAVN.Service.AdminAPI.Models.Customers.Enums.CustomerActivityStatus;
using PublicAddressStatus = MAVN.Service.AdminAPI.Models.Customers.Enums.PublicAddressStatus;

namespace MAVN.Service.AdminAPI.Controllers
{
    [ApiController]
    [LykkeAuthorizeWithoutCache]
    [Permission(PermissionType.Customers, PermissionLevel.View)]
    [Route("/api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private const int BatchCustomersCount = 100;

        private readonly ICustomerProfileClient _customerProfileClient;
        private readonly IOperationsHistoryClient _operationsHistoryClient;
        private readonly IReferralService _referralService;
        private readonly IPrivateBlockchainFacadeClient _pbfClient;
        private readonly ISettingsService _settingsService;
        private readonly IPrivateBlockchainFacadeClient _privateBlockchainFacadeClient;
        private readonly ICustomerManagementServiceClient _customerManagementServiceClient;
        private readonly IWalletManagementClient _walletManagementClient;
        private readonly ICrossChainWalletLinkerClient _crossChainWalletLinkerClient;
        private readonly IHistoryConverter _historyConverter;
        private readonly ICampaignClient _campaignClient;
        private readonly IMapper _mapper;

        public CustomersController(
            ICustomerProfileClient customerProfileClient,
            IOperationsHistoryClient operationsHistoryClient,
            IReferralService referralService,
            IPrivateBlockchainFacadeClient pbfClient,
            ISettingsService settingsService,
            IPrivateBlockchainFacadeClient privateBlockchainFacadeClient,
            ICustomerManagementServiceClient customerManagementServiceClient,
            IWalletManagementClient walletManagementClient,
            ICrossChainWalletLinkerClient crossChainWalletLinkerClient,
            IHistoryConverter historyConverter,
            ICampaignClient campaignClient,
            IMapper mapper)
        {
            _customerProfileClient = customerProfileClient;
            _operationsHistoryClient = operationsHistoryClient;
            _referralService = referralService;
            _pbfClient = pbfClient;
            _settingsService = settingsService;
            _privateBlockchainFacadeClient = privateBlockchainFacadeClient ??
                                             throw new ArgumentNullException(nameof(privateBlockchainFacadeClient));
            _customerManagementServiceClient = customerManagementServiceClient ??
                throw new ArgumentNullException(nameof(customerManagementServiceClient));
            _walletManagementClient = walletManagementClient ??
                throw new ArgumentNullException(nameof(walletManagementClient));
            _crossChainWalletLinkerClient = crossChainWalletLinkerClient ??
                throw new ArgumentNullException(nameof(crossChainWalletLinkerClient));
            _historyConverter = historyConverter;
            _campaignClient = campaignClient;
            _mapper = mapper;
        }

        /// <summary>
        /// Returns specific page of customer list.
        /// </summary>
        /// <param name="model">The pagination request parameters.</param>
        /// <returns>
        /// A collection of customers.
        /// </returns>
        /// <response code="200">A collection of customers.</response>
        /// <response code="400">An error occurred while getting customers.</response>
        [HttpPost("search")]
        [ProducesResponseType(typeof(CustomerListResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<CustomerListResponse> GetCustomersAsync([FromBody] CustomerListRequest model)
        {
            if (!string.IsNullOrEmpty(model.SearchValue))
            {
                CustomerProfileResponse response = null;

                if (model.SearchValue.IsValidEmail())
                    response = await _customerProfileClient.CustomerProfiles.GetByEmailAsync(
                        new GetByEmailRequestModel { Email = model.SearchValue, IncludeNotVerified = true });
                else if (Guid.TryParse(model.SearchValue, out Guid customerId))
                    response = await _customerProfileClient.CustomerProfiles.GetByCustomerIdAsync(customerId.ToString(), true);

                var customers = response?.Profile == null
                    ? new List<CustomerModel>()
                    : new List<CustomerModel> { _mapper.Map<CustomerModel>(response.Profile) };

                if (customers.Any())
                    await SetCustomersStatuses(customers);

                return new CustomerListResponse
                {
                    Customers = customers,
                    PagedResponse = new PagedResponseModel(1, response?.Profile == null ? 0 : 1)
                };
            }

            var result = await _customerProfileClient.CustomerProfiles.GetCustomersPaginatedAsync(
                new PaginationModel { CurrentPage = model.CurrentPage, PageSize = model.PageSize }, true);

            var resultCustomers = _mapper.Map<List<CustomerModel>>(result.Customers);

            foreach (var batch in resultCustomers.Batch(BatchCustomersCount))
            {
                await SetCustomersStatuses(batch.ToList());
            }

            return new CustomerListResponse
            {
                Customers = resultCustomers,
                PagedResponse = new PagedResponseModel(result.CurrentPage, result.TotalCount)
            };
        }

        /// <summary>
        /// Returns customer operations history.
        /// </summary>
        /// <param name="model">The paginated request parameters.</param>
        /// <returns>
        /// A collection of operations.
        /// </returns>
        [HttpGet("history")]
        [Permission(PermissionType.Customers, PermissionLevel.View)]
        [ProducesResponseType(typeof(CustomerOperationsHistoryResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<CustomerOperationsHistoryResponse> GetOperationsHistoryByIdAsync([FromQuery] CustomerOperationsHistoryRequest model)
        {
            var customerOperationsResponse = await _operationsHistoryClient.OperationsHistoryApi.GetByCustomerIdAsync(model.CustomerId,
                new MAVN.Service.OperationsHistory.Client.Models.Requests.PaginationModel
                {
                    CurrentPage = model.CurrentPage,
                    PageSize = model.PageSize
                });

            if (customerOperationsResponse.TotalCount == 0)
            {
                var customerProfileResponse = await _customerProfileClient.CustomerProfiles.GetByCustomerIdAsync(model.CustomerId, true);

                if (customerProfileResponse?.Profile == null)
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.CustomerNotFound);
            }

            var response = new CustomerOperationsHistoryResponse
            {
                PagedResponse = new PagedResponseModel(model.CurrentPage, customerOperationsResponse.TotalCount)
            };

            var operations = new List<CustomerOperationModel>();

            operations.AddRange(
                _mapper.Map<List<CustomerOperationModel>>(
                    _historyConverter.FromTransfers(model.CustomerId, customerOperationsResponse.Transfers)));
            operations.AddRange(
                _mapper.Map<List<CustomerOperationModel>>(
                    _historyConverter.FromPartnersPayments(customerOperationsResponse.PartnersPayments)));
            operations.AddRange(
                _mapper.Map<List<CustomerOperationModel>>(
                    _historyConverter.FromRefundedPartnersPayments(model.CustomerId, customerOperationsResponse.RefundedPartnersPayments)));
            operations.AddRange(
                _mapper.Map<List<CustomerOperationModel>>(
                    _historyConverter.FromBonusCashIns(customerOperationsResponse.BonusCashIns)));
            operations.AddRange(
                _mapper.Map<List<CustomerOperationModel>>(
                    _historyConverter.FromReferralStakes(customerOperationsResponse.ReferralStakes)));
            operations.AddRange(
                _mapper.Map<List<CustomerOperationModel>>(
                    _historyConverter.FromReleasedReferralStakes(customerOperationsResponse.ReleasedReferralStakes)));
            operations.AddRange(
                _mapper.Map<List<CustomerOperationModel>>(
                    _historyConverter.FromLinkedWalletTransfers(model.CustomerId, customerOperationsResponse.LinkedWalletTransfers)));
            operations.AddRange(
                _mapper.Map<List<CustomerOperationModel>>(
                    _historyConverter.FromFeeCollectedOperations(customerOperationsResponse.FeeCollectedOperations)));

            if (customerOperationsResponse.VoucherPurchasePayments != null)
            {
                var spendRuleIdentifiers = customerOperationsResponse.VoucherPurchasePayments
                    .Select(o => o.SpendRuleId)
                    .Distinct()
                    .ToList();

                var spendRuleNames = new Dictionary<Guid, string>();

                foreach (var spendRuleId in spendRuleIdentifiers)
                {
                    var spendRule = await _campaignClient.History.GetBurnRuleByIdAsync(spendRuleId);

                    if (spendRule != null)
                        spendRuleNames.Add(spendRuleId, spendRule.Title);
                }

                operations.AddRange(
                    _mapper.Map<List<CustomerOperationModel>>(
                        _historyConverter.FromVoucherPurchasePayments(
                            customerOperationsResponse.VoucherPurchasePayments, spendRuleNames)));
            }

            operations = operations.OrderByDescending(x => x.Timestamp).ToList();

            response.Operations = operations;

            return response;
        }

        /// <summary>
        /// Returns customer data
        /// </summary>
        /// <param name="customerId">Customer Id</param>
        /// <returns><see cref="CustomerModel"/></returns>
        [HttpGet("query")]
        [ProducesResponseType(typeof(CustomerDetailsModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<CustomerDetailsModel> GetCustomerByIdAsync([Required] [FromQuery] string customerId)
        {
            var parsedCustomerId = Guid.TryParse(customerId, out var customerIdGuid);

            if (!parsedCustomerId)
                throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.CustomerNotFound);

            var customerProfileResponseTask =
                await _customerProfileClient.CustomerProfiles.GetByCustomerIdAsync(customerId, true);

            if (customerProfileResponseTask.Profile == null)
                throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.CustomerNotFound);

            var customer = _mapper.Map<CustomerDetailsModel>(customerProfileResponseTask.Profile);
            
            if (customerProfileResponseTask.Profile.IsEmailVerified &&
                (_settingsService.IsPhoneVerificationDisabled() || customerProfileResponseTask.Profile.IsPhoneVerified))
            {
                var customerStatusTask =
                    _customerManagementServiceClient.CustomersApi.GetCustomerBlockStateAsync(customerId);

                var walletStatusTask =
                    _walletManagementClient.Api.GetCustomerWalletBlockStateAsync(customerId);

                await Task.WhenAll(
                    customerStatusTask,
                    walletStatusTask);

                if (customerStatusTask.Result.Error == CustomerBlockStatusError.CustomerNotFound)
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.CustomerNotFound);

                if (walletStatusTask.Result.Error == CustomerWalletBlockStatusError.CustomerNotFound)
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.CustomerNotFound);

                customer.WalletStatus =
                    _mapper.Map<Models.Customers.Enums.CustomerWalletActivityStatus>(walletStatusTask.Result.Status);
                customer.CustomerStatus =
                    _mapper.Map<CustomerActivityStatus>(customerStatusTask.Result.Status);
            }

            customer.ReferralCode = await _referralService.GetOrCreateReferralCodeAsync(customer.CustomerId);

            return customer;
        }

        /// <summary>
        /// Returns customer balance.
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        /// <returns>The current customer balance.</returns>
        /// <response code="200">The current customer balance.</response>
        /// <response code="400">An error occurred while getting customer balance.</response>
        /// <remarks>
        /// Error codes:
        /// - **InvalidCustomerId**
        /// - **CustomerWalletMissing**
        /// </remarks>
        [HttpGet("balance")]
        [ProducesResponseType(typeof(BalanceModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<BalanceModel> GetBalanceAsync([FromQuery] string customerId)
        {
            var asset = _settingsService.GetTokenName();
            var isCustomerIdValidGuid = Guid.TryParse(customerId, out var customerIdAsGuid);

            if (!isCustomerIdValidGuid)
                throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.InvalidCustomerId);

            var response = await _pbfClient.CustomersApi.GetBalanceAsync(customerIdAsGuid);

            if (response.Error != CustomerBalanceError.None)
            {
                switch (response.Error)
                {
                    case CustomerBalanceError.CustomerWalletMissing:
                        throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.CustomerWalletNotFound);
                    case CustomerBalanceError.InvalidCustomerId:
                        throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.InvalidCustomerId);
                }
            }

            return new BalanceModel
            {
                Asset = asset,
                Amount = response.Total
            };
        }

        /// <summary>
        /// Returns customer wallet address
        /// </summary>
        /// <param name="customerId">Customer Id</param>
        /// <returns><see cref="CustomerPrivateWalletAddressResponse"/></returns>
        [HttpGet("walletAddress")]
        [ProducesResponseType(typeof(CustomerPrivateWalletAddressResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<CustomerPrivateWalletAddressResponse> GetPrivateWalletAddressAsync(
            [Required] [FromQuery] string customerId)
        {
            var customerWalletAddressResponse =
                await _privateBlockchainFacadeClient.CustomersApi.GetWalletAddress(Guid.Parse(customerId));

            if (customerWalletAddressResponse.Error != CustomerWalletAddressError.None)
            {
                switch (customerWalletAddressResponse.Error)
                {
                    case CustomerWalletAddressError.CustomerWalletMissing:
                        throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.CustomerWalletNotFound);
                    case CustomerWalletAddressError.InvalidCustomerId:
                        throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.InvalidCustomerId);
                }
            }

            return _mapper.Map<CustomerPrivateWalletAddressResponse>(customerWalletAddressResponse);
        }

        /// <summary>
        /// Returns customer wallet address
        /// </summary>
        /// <param name="customerId">Customer Id</param>
        /// <returns><see cref="CustomerPublicWalletAddressResponse"/></returns>
        [HttpGet("publicWalletAddress")]
        [ProducesResponseType(typeof(CustomerPublicWalletAddressResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<CustomerPublicWalletAddressResponse> GetPublicWalletAddressAsync(
            [Required] [FromQuery] string customerId)
        {
            if (_settingsService.IsPublicBlockchainFeatureDisabled())
                return new CustomerPublicWalletAddressResponse { Status = PublicAddressStatus.NotLinked };

            var customerPublicWalletAddressResponse =
                await _crossChainWalletLinkerClient.CustomersApi.GetLinkedPublicAddressAsync(Guid.Parse(customerId));

            if (customerPublicWalletAddressResponse.Error != PublicAddressError.None)
            {
                switch (customerPublicWalletAddressResponse.Error)
                {
                    case PublicAddressError.InvalidCustomerId:
                        throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.InvalidCustomerId);
                }
            }

            return _mapper.Map<CustomerPublicWalletAddressResponse>(customerPublicWalletAddressResponse);
        }

        /// <summary>
        /// Block customer's access
        /// </summary>
        /// <param name="customerId">Customer's id</param>
        /// <response code="204">Customer blocked successfully</response>
        /// <response code="400">An error occurred while blocking a customer.</response>
        [HttpPost("block")]
        [Permission(PermissionType.Customers, PermissionLevel.Edit)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task BlockCustomer([FromQuery] string customerId)
        {
            try
            {
                var response = await _customerManagementServiceClient.CustomersApi.CustomerBlockAsync(new CustomerBlockRequest
                {
                    CustomerId = customerId
                });

                if (response.Error != CustomerBlockError.None)
                {
                    switch (response.Error)
                    {
                        case CustomerBlockError.CustomerNotFound:
                            throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.CustomerNotFound);
                        case CustomerBlockError.CustomerAlreadyBlocked:
                            throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.CustomerAlreadyBlocked);
                    }
                }
            }
            catch (ClientApiException exception)
            {
                throw new ValidationApiException(exception.ErrorResponse);
            }
        }

        /// <summary>
        /// Unblock customer's access
        /// </summary>
        /// <param name="customerId">Customer's id</param>
        /// <response code="204">Customer unblocked successfully</response>
        /// <response code="400">An error occurred while unblocking a customer.</response>
        [HttpPost("unblock")]
        [Permission(PermissionType.Customers, PermissionLevel.Edit)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task UnblockCustomer([FromQuery] string customerId)
        {
            try
            {
                var response = await _customerManagementServiceClient.CustomersApi.CustomerUnblockAsync(new CustomerUnblockRequest
                {
                    CustomerId = customerId
                });

                if (response.Error != CustomerUnblockError.None)
                {
                    switch (response.Error)
                    {
                        case CustomerUnblockError.CustomerNotFound:
                            throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.CustomerNotFound);
                        case CustomerUnblockError.CustomerNotBlocked:
                            throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.CustomerNotBlocked);
                    }
                }
            }
            catch (ClientApiException exception)
            {
                throw new ValidationApiException(exception.ErrorResponse);
            }
        }

        /// <summary>
        /// Block customer's wallet
        /// </summary>
        /// <param name="customerId">Customer's id</param>
        /// <response code="204">Customer's wallet blocked successfully</response>
        /// <response code="400">An error occurred while blocking a customer's wallet.</response>
        [HttpPost("blockWallet")]
        [Permission(PermissionType.Customers, PermissionLevel.Edit)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task BlockWallet([FromQuery] string customerId)
        {
            try
            {
                var response = await _walletManagementClient.Api.CustomerWalletBlockAsync(new CustomerWalletBlockRequest
                {
                    CustomerId = customerId
                });

                if (response.Error != CustomerWalletBlockError.None)
                {
                    switch (response.Error)
                    {
                        case CustomerWalletBlockError.CustomerNotFound:
                            throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.CustomerNotFound);
                        case CustomerWalletBlockError.CustomerWalletAlreadyBlocked:
                            throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.CustomerWalletAlreadyBlocked);
                    }
                }
            }
            catch (ClientApiException exception)
            {
                throw new ValidationApiException(exception.ErrorResponse);
            }
        }

        /// <summary>
        /// Unblock customer's wallet
        /// </summary>
        /// <param name="customerId">Customer's id</param>
        /// <response code="204">Customer's wallet unblocked successfully</response>
        /// <response code="400">An error occurred while unblocking a customer's wallet.</response>
        [HttpPost("unblockWallet")]
        [Permission(PermissionType.Customers, PermissionLevel.Edit)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task UnblockWallet([FromQuery] string customerId)
        {
            try
            {
                var response = await _walletManagementClient.Api.CustomerWalletUnblockAsync(new CustomerWalletUnblockRequest
                {
                    CustomerId = customerId
                });

                if (response.Error != CustomerWalletUnblockError.None)
                {
                    switch (response.Error)
                    {
                        case CustomerWalletUnblockError.CustomerNotFound:
                            throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.CustomerNotFound);
                        case CustomerWalletUnblockError.CustomerWalletNotBlocked:
                            throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.CustomerWalletNotBlocked);
                    }
                }
            }
            catch (ClientApiException exception)
            {
                throw new ValidationApiException(exception.ErrorResponse);
            }
        }

        private async Task SetCustomersStatuses(IReadOnlyList<CustomerModel> customers)
        {
            var stringIds = new List<string>();
            var validGuids = new List<Guid>();

            foreach (var cust in customers)
            {
                stringIds.Add(cust.CustomerId);

                if (Guid.TryParse(cust.CustomerId, out var guidId))
                {
                    validGuids.Add(guidId);
                }
            }

            var result = await _customerManagementServiceClient.CustomersApi.GetBatchOfCustomersBlockStatusAsync(new BatchOfCustomerStatusesRequest()
            {
                CustomerIds = stringIds.ToArray()
            });

            foreach (var customer in customers)
            {
                if (result.CustomersBlockStatuses.ContainsKey(customer.CustomerId))
                {
                    customer.CustomerStatus = _mapper.Map<CustomerActivityStatus>(result.CustomersBlockStatuses[customer.CustomerId]);
                }
            }
        }
    }
}
