using System.Threading.Tasks;

namespace MAVN.Service.AdminAPI.Domain.Services
{
    public interface ICredentialsGeneratorService
    {
        Task<string> GenerateRandomPasswordForAdminAsync();
    }
}
