using System.Collections.Generic;
using System.Threading.Tasks;
using Falcon.Common.Middleware.Authentication;
using MAVN.Service.AdminAPI.Domain.Enums;

namespace MAVN.Service.AdminAPI.Infrastructure
{
    public interface IExtRequestContext : IRequestContext
    {
        string SessionToken { get; }
        string UserId { get; }

        Task<bool> AdminHasPermissionAsync(IReadOnlyList<PermissionType> types, IReadOnlyList<PermissionLevel> levels);

        Task<PermissionLevel?> GetPermissionLevelAsync(PermissionType permissionType);
    }
}
