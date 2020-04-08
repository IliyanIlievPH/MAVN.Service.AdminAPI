using System.Threading.Tasks;
using Lykke.Service.Credentials.Client;
using Lykke.Service.Credentials.Client.Models.Requests;
using MAVN.Service.AdminAPI.Domain.Services;

namespace MAVN.Service.AdminAPI.DomainServices
{
    public class CredentialsGeneratorService : ICredentialsGeneratorService
    {
        private readonly int _suggestedAdminPasswordLength;
        private readonly ICredentialsClient _credentialsClient;

        public CredentialsGeneratorService(
            int suggestedAdminPasswordLength,
            ICredentialsClient credentialsClient)
        {
            _suggestedAdminPasswordLength = suggestedAdminPasswordLength;
            _credentialsClient = credentialsClient;
        }

        public async Task<string> GenerateRandomPasswordForAdminAsync()
        {
            var credentials = await _credentialsClient.Api.GenerateClientSecretAsync(
                new GenerateClientSecretRequest
                {
                    Length = _suggestedAdminPasswordLength
                });

            return credentials;
        }
    }
}
