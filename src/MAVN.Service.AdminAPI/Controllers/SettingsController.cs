using System;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Falcon.Common.Middleware.Authentication;
using Falcon.Numerics;
using Lykke.Common.ApiLibrary.Exceptions;
using Lykke.Service.AgentManagement.Client;
using Lykke.Service.AgentManagement.Client.Models.Requirements;
using Lykke.Service.CrossChainTransfers.Client;
using Lykke.Service.CrossChainTransfers.Client.Models.Enums;
using Lykke.Service.CrossChainTransfers.Client.Models.Requests;
using Lykke.Service.CrossChainWalletLinker.Client;
using Lykke.Service.CrossChainWalletLinker.Client.Models;
using Lykke.Service.CurrencyConvertor.Client;
using MAVN.Service.AdminAPI.Domain.Enums;
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
        private readonly IAgentManagementClient _agentManagementClient;
        private readonly ICrossChainWalletLinkerClient _crossChainWalletLinkerClient;
        private readonly ICrossChainTransfersClient _crossChainTransfersClient;

        private readonly IMapper _mapper;

        public SettingsController(
            ICurrencyConvertorClient currencyConverterClient,
            IAgentManagementClient agentManagementClient,
            ICrossChainWalletLinkerClient crossChainWalletLinkerClient,
            ICrossChainTransfersClient crossChainTransfersClient,
            IMapper mapper)
        {
            _currencyConverterClient = currencyConverterClient;
            _mapper = mapper;
            _agentManagementClient = agentManagementClient;
            _crossChainWalletLinkerClient = crossChainWalletLinkerClient;
            _crossChainTransfersClient = crossChainTransfersClient;
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
        public Task UpdateGlobalCurrencyRateAsync([FromBody] GlobalCurrencyRateModel model)
        {
            var request = _mapper.Map<Lykke.Service.CurrencyConvertor.Client.Models.Requests.GlobalCurrencyRateRequest>(model);

            return _currencyConverterClient.GlobalCurrencyRates.UpdateAsync(request);
        }

        /// <summary>
        /// Returns agents requirements.
        /// </summary>
        /// <returns>
        /// A global currency rate
        /// </returns>
        /// <response code="200">Agents requirements.</response>
        [HttpGet("agentRequirements")]
        [Permission(PermissionType.Settings, PermissionLevel.View)]
        [ProducesResponseType(typeof(AgentRequirementResponse), (int)HttpStatusCode.OK)]
        public async Task<AgentRequirementResponse> GetAgentRequirementsAsync()
        {
            var agentRequirements = await _agentManagementClient.Requirements.GetTokensRequirementsAsync();

            return _mapper.Map<AgentRequirementResponse>(agentRequirements);
        }

        /// <summary>
        /// Updates agents requirements.
        /// </summary>
        /// <response code="204">Agents requirements successfully updated.</response>
        [HttpPut("agentRequirements")]
        [Permission(PermissionType.Settings, PermissionLevel.Edit)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public Task UpdateAgentRequirementsAsync([FromBody] AgentRequirementUpdateRequest model)
        {
            var request = _mapper.Map<UpdateTokensRequirementModel>(model);

            return _agentManagementClient.Requirements.UpdateTokensRequirementAsync(request);
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
        }
    }
}
