using System.Threading.Tasks;
using MAVN.Service.AdminAPI.Domain.Enums;

namespace MAVN.Service.AdminAPI.Domain.Services
{
    public interface IAuditLogPublisher
    {
        Task PublishAuditLogAsync(string adminId, string jsonContext, ActionType actionType);
    }
}
