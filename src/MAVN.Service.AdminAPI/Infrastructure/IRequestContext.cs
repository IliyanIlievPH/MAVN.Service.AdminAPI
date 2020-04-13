using System.Collections.Generic;
using System.Threading.Tasks;
using MAVN.Service.AdminAPI.Domain.Enums;

namespace MAVN.Service.AdminAPI.Infrastructure
{
    public interface IRequestContext
    {
        string SessionToken { get; }
        string UserId { get; }

        Task<bool> AdminHasPermissionAsync(IReadOnlyList<PermissionType> types, PermissionLevel level);

        Task<PermissionLevel?> GetPermissionLevelAsync(PermissionType permissionType);
    }
}
