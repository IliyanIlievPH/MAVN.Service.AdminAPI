using System.Net;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.ApiLibrary.Contract;
using Lykke.Common.ApiLibrary.Exceptions;
using Lykke.Common.Log;
using MAVN.Service.AdminAPI.Models.Emails;
using MAVN.Service.AdminManagement.Client;
using MAVN.Service.AdminManagement.Client.Models.Enums;
using MAVN.Service.AdminManagement.Client.Models.Requests.Verification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MAVN.Service.AdminAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailsController : ControllerBase
    {
        private readonly ILog _log;
        private readonly IAdminManagementServiceClient _adminManagementServiceClient;

        public EmailsController(
            IAdminManagementServiceClient adminManagementServiceClient,
            ILogFactory logFactory)
        {
            _log = logFactory.CreateLog(this);
            _adminManagementServiceClient = adminManagementServiceClient;
        }

        /// <summary>
        /// Verifies email for the user in the system.
        /// </summary>
        /// <param name="model">Email verification request model</param>
        /// <remarks>
        /// Error codes:
        /// - **EmailIsAlreadyVerified**
        /// - **VerificationCodeDoesNotExist**
        /// - **VerificationCodeMismatch**
        /// - **VerificationCodeExpired**
        /// </remarks>
        [HttpPost("verify-email")]
        [AllowAnonymous]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(LykkeApiErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task VerifyEmailAsync([FromBody] EmailVerificationRequest model)
        {
            var result = await _adminManagementServiceClient.AdminsApi.ConfirmEmailAsync(new VerificationCodeConfirmationRequestModel
            {
                VerificationCode = model.VerificationCode
            });

            if (result.Error != VerificationCodeError.None)
            {
                switch (result.Error)
                {
                    case VerificationCodeError.AlreadyVerified:
                        _log.Warning(result.Error.ToString());
                        throw LykkeApiErrorException.BadRequest(
                            new LykkeApiErrorCode("EmailIsAlreadyVerified", "Email has been already verified"));
                    case VerificationCodeError.VerificationCodeDoesNotExist:
                        _log.Warning(result.Error.ToString());
                        throw LykkeApiErrorException.BadRequest(
                            new LykkeApiErrorCode(result.Error.ToString(), "Verification code does not exist"));
                    case VerificationCodeError.VerificationCodeMismatch:
                        _log.Warning(result.Error.ToString());
                        throw LykkeApiErrorException.BadRequest(
                            new LykkeApiErrorCode(result.Error.ToString(), "Verification code mismatch"));
                    case VerificationCodeError.VerificationCodeExpired:
                        _log.Warning(result.Error.ToString());
                        throw LykkeApiErrorException.BadRequest(
                            new LykkeApiErrorCode(result.Error.ToString(), "Verification code has expired"));
                }
            }

            _log.Info($"Email verification success with code '{model.VerificationCode}'");
        }
    }
}
