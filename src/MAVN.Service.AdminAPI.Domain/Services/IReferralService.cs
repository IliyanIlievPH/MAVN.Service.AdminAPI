using System.Threading.Tasks;

namespace MAVN.Service.AdminAPI.Domain.Services
{
    public interface IReferralService
    {
        Task<string> GetOrCreateReferralCodeAsync(string customerId);
    }
}
