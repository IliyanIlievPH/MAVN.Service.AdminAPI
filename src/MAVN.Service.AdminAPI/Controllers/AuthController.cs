using System;
using System.Collections.Concurrent;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Common;
using Falcon.Common.Middleware.Authentication;
using Lykke.Common.ApiLibrary.Exceptions;
using Lykke.Service.Sessions.Client;
using MAVN.Service.AdminAPI.Domain.Enums;
using MAVN.Service.AdminAPI.Domain.Services;
using MAVN.Service.AdminAPI.Infrastructure.Constants;
using MAVN.Service.AdminAPI.Models.Admins;
using MAVN.Service.AdminAPI.Models.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using IRequestContext = MAVN.Service.AdminAPI.Infrastructure.IRequestContext;

namespace MAVN.Service.AdminAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly ISessionsServiceClient _sessionsServiceClient;
        private readonly IAdminsService _adminsService;
        private readonly IRequestContext _requestContext;
        private readonly IMapper _mapper;

        private static readonly ConcurrentDictionary<string, DateTime> _logoutSessionTokens = new ConcurrentDictionary<string, DateTime>();

        public AuthController(
            ISessionsServiceClient sessionsServiceClient,
            IRequestContext requestContext,
            IAdminsService adminsService, 
            IMapper mapper)
        {
            _sessionsServiceClient = sessionsServiceClient;
            _requestContext = requestContext ?? throw new ArgumentNullException(nameof(requestContext));
            _adminsService = adminsService;
            _mapper = mapper;
        }

        /// <summary>
        /// Login admin user.
        /// </summary>
        /// <remarks>
        /// Error codes:
        /// - **InvalidEmailFormat**
        /// - **InvalidCredentials**
        /// </remarks>
        [HttpPost("login")]
        [AllowAnonymous]
        [SwaggerOperation("Login")]
        [ProducesResponseType(typeof(LoginResponseModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        public async Task<IActionResult> Login([FromBody]LoginModel model)
        {
            if (!model.Email.IsValidEmailAndRowKey())
                throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.InvalidEmailFormat);

            var (error, admin, token) = await _adminsService.AuthenticateAsync(model.Email, model.Password);
            
            switch (error)
            {
                case AdminServiceCreateResponseError.None:
                    return Ok(new LoginResponseModel
                    {
                        Token = token,
                        AdminUser = _mapper.Map<AdminModel>(admin)
                    });
                case AdminServiceCreateResponseError.AdminNotActive:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.AdminNotActive);
                case AdminServiceCreateResponseError.LoginNotFound:
                case AdminServiceCreateResponseError.PasswordMismatch:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.InvalidCredentials);
                case AdminServiceCreateResponseError.InvalidEmailOrPasswordFormat:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.InvalidEmailOrPasswordFormat);
                default:
                    throw new InvalidOperationException($"Unexpected error during Authenticate for {model.Email.SanitizeEmail()} - {error}");
            }
        }
        
        /// <summary>
        /// Change admin credentials.
        /// </summary>
        /// <remarks>
        /// Error codes:
        /// - **AdminNotActive**
        /// - **InvalidCredentials**
        /// - **InvalidEmailOrPasswordFormat**
        /// - **NewPasswordInvalid**
        /// </remarks>
        [HttpPost("changePassword")]
        [LykkeAuthorizeWithoutCache]
        [SwaggerOperation("ChangePassword")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        public async Task<IActionResult> ChangePassword([FromBody]ChangePasswordModel model)
        {
            var (adminServiceResponseError, admin) = await _adminsService.GetAsync(_requestContext.UserId);

            var email = admin.Email;

            var error = await _adminsService.ChangePasswordAsync(email, model.CurrentPassword, model.NewPassword);
            
            switch (error)
            {
                case AdminChangePasswordErrorCodes.None:
                    return Ok();
                case AdminChangePasswordErrorCodes.AdminNotActive:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.AdminNotActive);
                case AdminChangePasswordErrorCodes.LoginNotFound:
                case AdminChangePasswordErrorCodes.PasswordMismatch:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.InvalidCredentials);
                case AdminChangePasswordErrorCodes.InvalidEmailOrPasswordFormat:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.InvalidEmailOrPasswordFormat);
                case AdminChangePasswordErrorCodes.NewPasswordInvalid:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.NewPasswordInvalid);
                default:
                    throw new InvalidOperationException($"Unexpected error during change password for {email.SanitizeEmail()} - {error}");
            }
        }

        /// <summary>
        /// Change admin credentials.
        /// </summary>
        /// <remarks>
        /// Error codes:
        /// - **AdminNotActive**
        /// - **InvalidCredentials**
        /// - **InvalidEmailOrPasswordFormat**
        /// - **NewPasswordInvalid**
        /// </remarks>
        [HttpPost("changePasswordAnonymous")]
        [AllowAnonymous]
        [SwaggerOperation("ChangePasswordAnonymous")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        public async Task<IActionResult> ChangePasswordAnonymous([FromBody]ChangePasswordAnonymousModel model)
        {
            if (!model.Email.IsValidEmailAndRowKey())
                throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.InvalidEmailFormat);

            var error = await _adminsService.ChangePasswordAsync(model.Email, model.CurrentPassword, model.NewPassword);
            
            switch (error)
            {
                case AdminChangePasswordErrorCodes.None:
                    return Ok();
                case AdminChangePasswordErrorCodes.AdminNotActive:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.AdminNotActive);
                case AdminChangePasswordErrorCodes.LoginNotFound:
                case AdminChangePasswordErrorCodes.PasswordMismatch:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.InvalidCredentials);
                case AdminChangePasswordErrorCodes.InvalidEmailOrPasswordFormat:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.InvalidEmailOrPasswordFormat);
                case AdminChangePasswordErrorCodes.NewPasswordInvalid:
                    throw LykkeApiErrorException.BadRequest(ApiErrorCodes.Service.NewPasswordInvalid);
                default:
                    throw new InvalidOperationException($"Unexpected error during change password for {model.Email.SanitizeEmail()} - {error}");
            }
        }

        /// <summary>
        /// Logout admin user.
        /// </summary>
        [HttpPost("decline-logout")]
        [LykkeAuthorizeWithoutCache]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public IActionResult DeclineLogout()
        {
            if (_logoutSessionTokens.ContainsKey(_requestContext.SessionToken))
            {
                _logoutSessionTokens.TryRemove(_requestContext.SessionToken, out var value);

#if DEBUG
                Console.WriteLine($"Logout declined: {_requestContext.SessionToken}");
#endif
            }

            return Ok();
        }

        /// <summary>
        /// Logout admin user.
        /// </summary>
        [HttpPost("logout")]
        [LykkeAuthorizeWithoutCache]
        [SwaggerOperation("logout")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public IActionResult Logout()
        {
            var token = _requestContext.SessionToken;
            _logoutSessionTokens.TryAdd(token, DateTime.Now);

            Task.Run(async () =>
            {
                await Task.Delay(10_000);

#if DEBUG
                Console.WriteLine($"Try to delete session: {token}");
#endif

                if (_logoutSessionTokens.ContainsKey(token))
                {
                    _logoutSessionTokens.TryRemove(token, out var value);

                    await _sessionsServiceClient.SessionsApi.DeleteSessionIfExistsAsync(token);

#if DEBUG
                    Console.WriteLine($"Session deleted: {token}");
#endif
                }
            });

#if DEBUG
            Console.WriteLine($"Logout called: {token}");
#endif

            return Ok();
        }
    }
}
