using System;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Common;
using MAVN.Common.Middleware.Authentication;
using MAVN.Numerics;
using Lykke.Common.ApiLibrary.Exceptions;
using MAVN.Service.CrossChainTransfers.Client;
using MAVN.Service.CrossChainTransfers.Client.Models.Enums;
using MAVN.Service.CrossChainTransfers.Client.Models.Requests;
using MAVN.Service.CrossChainWalletLinker.Client;
using MAVN.Service.CrossChainWalletLinker.Client.Models;
using MAVN.Service.CurrencyConvertor.Client;
using MAVN.Service.AdminAPI.Domain.Enums;
using MAVN.Service.AdminAPI.Domain.Services;
using MAVN.Service.AdminAPI.Infrastructure.Constants;
using MAVN.Service.AdminAPI.Infrastructure.CustomAttributes;
using MAVN.Service.AdminAPI.Models.Settings;
using Microsoft.AspNetCore.Mvc;

namespace MAVN.Service.AdminAPI.Controllers
{
    [ApiController]
    [LykkeAuthorizeWithoutCache]
    [Route("/api/[controller]")]
    public class SettingsController : ControllerBase
    {
        private readonly ICurrencyConvertorClient _currencyConverterClient;
        private readonly ICrossChainWalletLinkerClient _crossChainWalletLinkerClient;
        private readonly ICrossChainTransfersClient _crossChainTransfersClient;
        private readonly IAuditLogPublisher _auditLogPublisher;
        private readonly IRequestContext _requestContext;

        private readonly IMapper _mapper;

        public SettingsController(
            ICurrencyConvertorClient currencyConverterClient,
            ICrossChainWalletLinkerClient crossChainWalletLinkerClient,
            ICrossChainTransfersClient crossChainTransfersClient,
            IAuditLogPublisher auditLogPublisher,
            IRequestContext requestContext,
            IMapper mapper)
        {
            _currencyConverterClient = currencyConverterClient;
            _mapper = mapper;
            _crossChainWalletLinkerClient = crossChainWalletLinkerClient;
            _crossChainTransfersClient = crossChainTransfersClient;
            _auditLogPublisher = auditLogPublisher;
            _requestContext = requestContext;
        }

        /// <summary>
        /// Returns global currency rate.
        /// </summary>
        /// <returns>
        /// A global currency rate
        /// </returns>
        /// <response code="200">A global currency rate.</response>
        [HttpGet("globalCurrencyRate")]
        [Permission(
            new []
            {
                PermissionType.ActionRules,
                PermissionType.ProgramPartners,
                PermissionType.Settings
            },
            new[]
            {
                PermissionLevel.View,
                PermissionLevel.PartnerEdit,
            }
        )]
        [ProducesResponseType(typeof(GlobalCurrencyRateModel), (int)HttpStatusCode.OK)]
        public async Task<GlobalCurrencyRateModel> GetGlobalCurrencyRateAsync()
        {
            var globalCurrencyRate = await _currencyConverterClient.GlobalCurrencyRates.GetAsync();

            return _mapper.Map<GlobalCurrencyRateModel>(globalCurrencyRate);
        }

        /// <summary>
        /// Updates global currency rate.
        /// </summary>
        /// <response code="204">The global currency rate successfully updated.</response>
        [HttpPut("globalCurrencyRate")]
        [Permission(PermissionType.Settings, PermissionLevel.Edit)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task UpdateGlobalCurrencyRateAsync([FromBody] GlobalCurrencyRateModel model)
        {
            var request = _mapper.Map<MAVN.Service.CurrencyConvertor.Client.Models.Requests.GlobalCurrencyRateRequest>(model);

            await _auditLogPublisher.PublishAuditLogAsync(_requestContext.UserId, request.ToJson(), ActionType.UpdateGlobalCurrencyRate);

            await _currencyConverterClient.GlobalCurrencyRates.UpdateAsync(request);
        }

        /// <summary>
        /// Returns operation fees config.
        /// </summary>
        /// <returns>
        /// A operation fees config
        /// </returns>
        /// <response code="200">A operation fees config.</response>
        [HttpGet("operationFees")]
        [Permission(PermissionType.Settings, PermissionLevel.View)]
        [ProducesResponseType(typeof(OperationFeesModel), (int)HttpStatusCode.OK)]
        public async Task<OperationFeesModel> GetOperationFeesAsync()
        {
            var operationFees = new OperationFeesModel();

            var operationFeesConfig = await _crossChainWalletLinkerClient.ConfigurationApi.GetAllAsync();

            foreach (var model in operationFeesConfig)
            {
                switch (model.Type)
                {
                    case ConfigurationItemType.FirstTimeLinkingFee:
                        operationFees.FirstTimeLinkingFee = Money18.Parse(model.Value);
                        break;
                    case ConfigurationItemType.SubsequentLinkingFee:
                        operationFees.SubsequentLinkingFee = Money18.Parse(model.Value);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            var crossChainTransferFee = await _crossChainTransfersClient.FeesApi.GetTransferToPublicFeeAsync();

            operationFees.CrossChainTransferFee = crossChainTransferFee.Fee;

            return operationFees;
        }

        /// <summary>
        /// Returns operation fees config.
        /// </summary>
        /// <returns>
        /// A operation fees config
        /// </returns>
        /// <response code="200">A operation fees config.</response>
        [HttpPut("operationFees")]
        [Permission(PermissionType.Settings, PermissionLevel.Edit)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task UpdateOperationFeesAsync(OperationFeesModel model)
        {
            if (model.CrossChainTransferFee.HasValue)
            {
                var setTransferToPublicFeeResponse = await _crossChainTransfersClient.FeesApi.SetTransferToPublicFeeAsync(new SetTransferToPublicFeeRequest
                {
                    Fee = model.CrossChainTransferFee.Value
                });

                if (setTransferToPublicFeeResponse.Error != FeesErrorCodes.None)
                {
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.UnknownError);
                }
            }

            if (model.FirstTimeLinkingFee.HasValue)
            {
                var configurationItemUpdateResponseModel = await _crossChainWalletLinkerClient.ConfigurationApi.UpdateOrInsertItemAsync(new ConfigurationItemRequestModel
                {
                    Type = ConfigurationItemType.FirstTimeLinkingFee,
                    Value = model.FirstTimeLinkingFee.Value.ToString()
                });

                if (configurationItemUpdateResponseModel.Error != ConfigurationItemUpdateError.None)
                {
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.UnknownError);
                }
            }

            if (model.SubsequentLinkingFee.HasValue)
            {
                var configurationItemUpdateResponseModel = await _crossChainWalletLinkerClient.ConfigurationApi.UpdateOrInsertItemAsync(new ConfigurationItemRequestModel
                {
                    Type = ConfigurationItemType.SubsequentLinkingFee,
                    Value = model.SubsequentLinkingFee.Value.ToString()
                });

                if (configurationItemUpdateResponseModel.Error != ConfigurationItemUpdateError.None)
                {
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.UnknownError);
                }
            }

            await _auditLogPublisher.PublishAuditLogAsync(_requestContext.UserId, model.ToJson(), ActionType.UpdateOperationFees);
        }
    }
}
