using System.Threading.Tasks;
using MAVN.Service.Referral.Client;
using MAVN.Service.Referral.Client.Enums;
using MAVN.Service.Referral.Client.Models.Requests;
using MAVN.Service.AdminAPI.Domain.Services;

namespace MAVN.Service.AdminAPI.DomainServices
{
    public class ReferralService: IReferralService
    {
        private readonly IReferralClient _referralClient;

        public ReferralService(IReferralClient referralClient)
        {
            _referralClient = referralClient;
        }

        public async Task<string> GetOrCreateReferralCodeAsync(string customerId)
        {
            var referral = await _referralClient.ReferralApi.GetAsync(customerId);

            if (referral.ReferralCode != null)
            {
                return referral.ReferralCode;
            }

            if (referral.ReferralCode == null && referral.ErrorCode == ReferralErrorCodes.ReferralNotFound)
            {
                var referralCreate = await _referralClient.ReferralApi.PostAsync(new ReferralCreateRequest()
                {
                    CustomerId = customerId
                });

                return referralCreate.ReferralCode;
            }

            return null;
        }
    }
}
