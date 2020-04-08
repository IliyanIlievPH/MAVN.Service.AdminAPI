using System.Linq;
using Common.Log;
using Lykke.Common.ApiLibrary.Contract;
using Lykke.Common.ApiLibrary.Exceptions;
using Lykke.Common.Log;
using MAVN.Service.AdminAPI.Infrastructure.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MAVN.Service.AdminAPI.Infrastructure.LykkeApiError
{
    public static class InvalidModelStateResponseFactory
    {
        public static ILog Logger { get; set; }

        /// <summary>
        ///     General validation error processing delegate.
        ///     Wraps any failed model validation into <see cref="LykkeApiErrorResponse" />.
        ///     To return custom error code, throw <see cref="LykkeApiErrorException" /> from validator
        ///     with appropriate code from <see cref="ApiErrorCodes.ModelValidation" />.
        ///     If code does not exist feel free to create a new one.
        /// </summary>
        public static IActionResult CreateInvalidModelResponse(ActionContext context)
        {
            {
                var message = GetErrorMessage(context.ModelState);
                var apiErrorResponse = new LykkeApiErrorResponse
                {
                    Error = ApiErrorCodes.ModelValidation.ModelValidationFailed.Name,
                    Message = message
                };

                Logger.Warning($"ModelValidationFailed error: '{message}'");

                return new BadRequestObjectResult(apiErrorResponse)
                {
                    ContentTypes = { "application/json" }
                };
            }
        }

        private static string GetErrorMessage(ModelStateDictionary modelStateDictionary)
        {
            var modelError = modelStateDictionary?.Values.FirstOrDefault()?.Errors.FirstOrDefault();

            if (modelError == null)
                return string.Empty;

            return modelError.Exception != null
                ? modelError.Exception.Message
                : modelError.ErrorMessage;
        }
    }
}
