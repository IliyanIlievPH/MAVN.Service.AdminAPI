using System.Collections.Generic;
using System.Net;
using System.Numerics;
using System.Threading.Tasks;
using AutoMapper;
using Falcon.Common.Middleware.Authentication;
using Falcon.Numerics;
using Lykke.Service.QuorumExplorer.Client;
using Lykke.Service.QuorumExplorer.Client.Models;
using MAVN.Service.AdminAPI.Domain.Enums;
using MAVN.Service.AdminAPI.Infrastructure.CustomAttributes;
using MAVN.Service.AdminAPI.Models.Blockchain;
using MAVN.Service.AdminAPI.Models.Common;
using Microsoft.AspNetCore.Mvc;
using MoreLinq;

namespace MAVN.Service.AdminAPI.Controllers
{
    [ApiController]
    [Permission(PermissionType.BlockchainOperations, PermissionLevel.View)]
    [LykkeAuthorizeWithoutCache]
    [Route("/api/")]
    public class BlockchainController : Controller
    {
        private readonly List<string> _amountParamNames = new List<string>{ "amount", "value", "burntAmount" };
        private readonly IQuorumExplorerClient _quorumExplorerClient;
        private readonly IMapper _mapper;

        public BlockchainController(IQuorumExplorerClient quorumExplorerClient, IMapper mapper)
        {
            _quorumExplorerClient = quorumExplorerClient;
            _mapper = mapper;
        }

        /// <summary>
        /// Get paginated list of transactions.
        /// </summary>
        /// <remarks>
        /// Function. Return the list of transactions.
        /// </remarks>
        /// <returns>
        /// 200 - function done
        /// </returns>
        [HttpGet("transactions")]
        [ProducesResponseType(typeof(TransactionListResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<TransactionListResponse> GetTransactionsAsync([FromQuery] TransactionListRequest request)
        {
            var result =
                await _quorumExplorerClient.TransactionsApi.GetFilteredAsync(new FilteredTransactionsRequest
                {
                    FunctionName = request.FunctionName,
                    FunctionSignature = request.FunctionSignature,
                    From = request.From ?? new List<string>(),
                    To = request.To ?? new List<string>(),
                    AffectedAddresses = request.AffectedAddresses ?? new List<string>(),
                    PagingInfo = new PaginationModel
                    {
                        CurrentPage = request.PagedRequest.CurrentPage,
                        PageSize = request.PagedRequest.PageSize
                    }
                });

            return new TransactionListResponse
            {
                PagedResponse = new PagedResponseModel
                {
                    CurrentPage = result.CurrentPage,
                    TotalCount = result.TotalCount
                },
                Transactions = _mapper.Map<IReadOnlyCollection<TransactionModel>>(result.Transactions)
            };
        }

        /// <summary>
        /// Get transaction for given hash.
        /// </summary>
        /// <remarks>
        /// Function. Return the the transaction.
        /// </remarks>
        /// <returns>
        /// 200 - function done
        /// </returns>
        [HttpGet("transactions/hash")]
        [ProducesResponseType(typeof(TransactionModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<TransactionModel> GetTransactionByHashAsync([FromQuery] string hash)
        {
            var result = await _quorumExplorerClient.TransactionsApi.GetDetailsAsync(hash);

            return _mapper.Map<TransactionModel>(result.Transaction);
        }

        /// <summary>
        /// Get event for given transaction.
        /// </summary>
        /// <remarks>
        /// Function. Return the the events of the transaction.
        /// </remarks>
        /// <returns>
        /// 200 - function done
        /// </returns>
        [HttpGet("transactions/events")]
        [ProducesResponseType(typeof(EventListResponse), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<EventListResponse> GetTransactionEventsByHashAsync([FromQuery] string hash,
            [FromQuery] PaginationModel request)
        {
            var result = await _quorumExplorerClient.TransactionsApi.GetEventsByTransactionAsync(
                new TransactionEventsRequest
                {
                    TransactionHash = hash, 
                    CurrentPage = request.CurrentPage, 
                    PageSize = request.PageSize
                });

            return new EventListResponse
            {
                Events = _mapper.Map<IReadOnlyCollection<EventModel>>(result.Events),
                PagedResponse = new PagedResponseModel
                {
                    CurrentPage = result.CurrentPage, 
                    TotalCount = result.TotalCount
                }
            };
        }

        /// <summary>
        /// Get paginated list of events.
        /// </summary>
        /// <remarks>
        /// Function. Return the list of events.
        /// </remarks>
        /// <returns>
        /// 200 - function done
        /// </returns>
        [HttpGet("events")]
        [ProducesResponseType(typeof(EventListResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<EventListResponse> GetEventsAsync([FromQuery] EventListRequest request)
        {
            var result = await _quorumExplorerClient.EventsApi.GetFilteredAsync(new FilteredEventsRequest
            {
                EventName = request.EventName,
                EventSignature = request.EventSignature,
                Address = request.Address,
                AffectedAddresses = request.AffectedAddresses ?? new List<string>(),
                PagingInfo = new PaginationModel
                {
                    CurrentPage = request.PagedRequest.CurrentPage,
                    PageSize = request.PagedRequest.PageSize
                }
            });

            result.Events.ForEach(e =>
            {
                e.Parameters.ForEach(p =>
                {
                    if (p.Type == "uint256" && _amountParamNames.Contains(p.Name))
                    {
                        var money = Money18.CreateFromAtto(BigInteger.Parse(p.Value));

                        p.Value = money.ToString();
                    }
                });
            });

            return new EventListResponse
            {
                PagedResponse = new PagedResponseModel
                {
                    CurrentPage = result.CurrentPage,
                    TotalCount = result.TotalCount
                },
                Events = _mapper.Map<IReadOnlyCollection<EventModel>>(result.Events)
            };
        }
    }
}
