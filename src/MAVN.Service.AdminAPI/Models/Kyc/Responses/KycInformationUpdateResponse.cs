using MAVN.Service.AdminAPI.Models.Kyc.Enum;

namespace MAVN.Service.AdminAPI.Models.Kyc.Responses
{
    /// <summary>Response model for KYC update</summary>
    public class KycInformationUpdateResponse
    {
        /// <summary>Holds error code</summary>
        public UpdateKycErrorCodes Error { get; set; }
    }
}
