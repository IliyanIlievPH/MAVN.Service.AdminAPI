using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Falcon.Common.Middleware.Authentication;
using Lykke.Service.AdminManagement.Client;
using Lykke.Service.AdminManagement.Client.Models.Requests;
using Lykke.Service.Sessions.Client;
using MAVN.Service.AdminAPI.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace MAVN.Service.AdminAPI.Infrastructure
{
    public class RequestContext : IExtRequestContext
    {
        private readonly HttpContext _httpContext;
        private readonly ISessionsServiceClient _sessionsServiceClient;
        private readonly IAdminManagementServiceClient _adminManagementServiceClient;
        private readonly IMapper _mapper;
        private readonly Dictionary<PermissionType, PermissionLevel> _permissionsDict = new Dictionary<PermissionType, PermissionLevel>();

        private string _userId;

        public RequestContext(
            IHttpContextAccessor httpContextAccessor,
            ISessionsServiceClient sessionsServiceClient,
            IAdminManagementServiceClient adminManagementServiceClient,
            IMapper mapper)
        {
            _sessionsServiceClient = sessionsServiceClient;
            _httpContext = httpContextAccessor.HttpContext;
            _adminManagementServiceClient = adminManagementServiceClient;
            _mapper = mapper;
        }

        public string SessionToken => _httpContext.GetLykkeToken();

        public string UserId => GetUserId();

        public async Task<bool> AdminHasPermissionAsync(IReadOnlyList<PermissionType> types, PermissionLevel level)
        {
            var adminId = GetUserId();

            if (string.IsNullOrWhiteSpace(adminId))
                return false;

            await InitAdminPermissionsAsync(adminId);

            if (level == PermissionLevel.View || level == PermissionLevel.PartnerView)
                return types.Any(t => _permissionsDict.ContainsKey(t));

            foreach (var type in types)
            {
                if (!_permissionsDict.TryGetValue(type, out var permissionLevel))
                    continue;

                if (permissionLevel == PermissionLevel.Edit || permissionLevel == PermissionLevel.PartnerEdit)
                    return true;
            }

            return false;
        }

        public async Task<PermissionLevel?> GetPermissionLevelAsync(PermissionType permissionType)
        {
            if (_permissionsDict.Count == 0)
                await InitAdminPermissionsAsync(GetUserId());

            _permissionsDict.TryGetValue(permissionType, out var permissionLevel);

            return permissionLevel;
        }

        private string GetUserId()
        {
            if (_userId != null)
                return _userId;

            var token = _httpContext.GetLykkeToken();

            if (string.IsNullOrWhiteSpace(token))
                return null;

            var session = _sessionsServiceClient.SessionsApi.GetSessionAsync(token).GetAwaiter().GetResult();

            _userId = session?.ClientId;

            return _userId;
        }

        private async Task InitAdminPermissionsAsync(string adminId)
        {
            var adminPermissions = await _adminManagementServiceClient.AdminsApi.GetPermissionsAsync(
                new GetAdminByIdRequestModel
                {
                    AdminUserId = adminId
                });

            if (adminPermissions != null)
                foreach (var adminPermission in adminPermissions)
                {
                    _permissionsDict[Enum.Parse<PermissionType>(adminPermission.Type)] =
                        _mapper.Map<PermissionLevel>(adminPermission.Level);
                }
        }
    }
}
