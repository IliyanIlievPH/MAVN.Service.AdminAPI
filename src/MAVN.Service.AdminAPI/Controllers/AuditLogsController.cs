using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using MAVN.Common.Middleware.Authentication;
using MAVN.Service.AdminAPI.Domain.Enums;
using MAVN.Service.AdminAPI.Infrastructure.CustomAttributes;
using MAVN.Service.AdminAPI.Models.AuditLogs;
using MAVN.Service.AuditLogs.Client;
using Microsoft.AspNetCore.Mvc;
using GetAuditLogsRequest = MAVN.Service.AdminAPI.Models.AuditLogs.GetAuditLogsRequest;

namespace MAVN.Service.AdminAPI.Controllers
{
    [ApiController]
    [Permission(
        PermissionType.AuditLogs,
        new[]
        {
            PermissionLevel.View,
        }
    )]
    [LykkeAuthorizeWithoutCache]
    [Route("/api/[controller]")]
    public class AuditLogsController : ControllerBase
    {
        private readonly IAuditLogsClient _auditLogsClient;
        private readonly IMapper _mapper;

        public AuditLogsController(IAuditLogsClient auditLogsClient, IMapper mapper)
        {
            _auditLogsClient = auditLogsClient;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets paged audit logs
        /// </summary>
        /// <returns><see cref="GetAuditLogsResponse"/></returns>
        [HttpGet]
        [ProducesResponseType(typeof(GetAuditLogsResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<GetAuditLogsResponse> GetAuditLogsAsync([FromQuery] GetAuditLogsRequest request)
        {
            var result =
                await _auditLogsClient.Api.GetAuditLogsAsync(
                    _mapper.Map<AuditLogs.Client.Models.Requests.GetAuditLogsRequest>(request));

            return _mapper.Map<GetAuditLogsResponse>(result);
        }
    }
}
