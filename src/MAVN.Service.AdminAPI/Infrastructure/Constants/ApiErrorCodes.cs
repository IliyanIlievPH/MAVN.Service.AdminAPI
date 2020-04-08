using Lykke.Common.ApiLibrary.Contract;
using Lykke.Common.ApiLibrary.Exceptions;

namespace MAVN.Service.AdminAPI.Infrastructure.Constants
{
    /// <summary>
    ///     Class for storing all possible error codes that may happen in Api.
    ///     Use it with <see cref="LykkeApiErrorException" />.
    /// </summary>
    public static class ApiErrorCodes
    {
        /// <summary>
        ///     Group for client and service related error codes.
        /// </summary>
        public static class Service
        {
            /// <summary>
            /// Unknown error.
            /// </summary>
            public static readonly ILykkeApiErrorCode UnknownError =
                new LykkeApiErrorCode(nameof(UnknownError), "Unknown error.");

            /// <summary>
            ///     Invalid email format.
            /// </summary>
            public static readonly ILykkeApiErrorCode InvalidEmailFormat =
                new LykkeApiErrorCode(nameof(InvalidEmailFormat), "Invalid email format.");

            /// <summary>
            ///     Invalid email or password format.
            /// </summary>
            public static readonly ILykkeApiErrorCode InvalidEmailOrPasswordFormat =
                new LykkeApiErrorCode(nameof(InvalidEmailOrPasswordFormat), "Invalid email or password format.");

            /// <summary>
            ///     Login or password is not valid.
            /// </summary>
            public static readonly ILykkeApiErrorCode InvalidCredentials =
                new LykkeApiErrorCode(nameof(InvalidCredentials), "Login or password is not valid.");

            /// <summary>
            ///     Customer not found.
            /// </summary>
            public static readonly ILykkeApiErrorCode CustomerNotFound =
                new LykkeApiErrorCode(nameof(CustomerNotFound), "Customer not found.");

            /// <summary>
            ///     Asset not found.
            /// </summary>
            public static readonly ILykkeApiErrorCode AssetNotFound =
                new LykkeApiErrorCode(nameof(AssetNotFound), "Asset not found.");

            /// <summary>
            ///     Referral not found.
            /// </summary>
            public static readonly ILykkeApiErrorCode ReferralNotFound =
                new LykkeApiErrorCode(nameof(ReferralNotFound), "Referral not found.");

            /// <summary>
            ///     Customer wallet not found.
            /// </summary>
            public static readonly ILykkeApiErrorCode CustomerWalletNotFound =
                new LykkeApiErrorCode(nameof(CustomerWalletNotFound), "Customer wallet not found.");

            /// <summary>
            ///     Invalid customer id.
            /// </summary>
            public static readonly ILykkeApiErrorCode InvalidCustomerId =
                new LykkeApiErrorCode(nameof(InvalidCustomerId), "Invalid customer id.");

            /// <summary>
            ///     Invalid Burn Rule id.
            /// </summary>
            public static readonly ILykkeApiErrorCode InvalidBurnRuleId =
                new LykkeApiErrorCode(nameof(InvalidBurnRuleId), "Invalid Burn Rule id.");

            /// <summary>
            ///     The customer already is blocked
            /// </summary>
            public static readonly ILykkeApiErrorCode CustomerAlreadyBlocked =
                new LykkeApiErrorCode(nameof(CustomerAlreadyBlocked), "The customer already is blocked");

            /// <summary>
            ///     The customer is not blocked
            /// </summary>
            public static readonly ILykkeApiErrorCode CustomerNotBlocked =
                new LykkeApiErrorCode(nameof(CustomerNotBlocked), "The customer is not blocked.");

            /// <summary>
            ///     The customer's wallet already is blocked
            /// </summary>
            public static ILykkeApiErrorCode CustomerWalletAlreadyBlocked =
                new LykkeApiErrorCode(nameof(CustomerWalletAlreadyBlocked), "The customer's wallet already is blocked.");

            /// <summary>
            ///     The customer's wallet is not blocked
            /// </summary>
            public static ILykkeApiErrorCode CustomerWalletNotBlocked =
                new LykkeApiErrorCode(nameof(CustomerWalletNotBlocked), "The customer's wallet is not blocked.");

            /// <summary>
            ///     The given admin is already registered
            /// </summary>
            public static ILykkeApiErrorCode AdminAlreadyRegistered =
                new LykkeApiErrorCode(nameof(AdminAlreadyRegistered), "The given admin is already registered.");

            /// <summary>
            ///     The given admin is not found
            /// </summary>
            public static ILykkeApiErrorCode AdminNotFound =
                new LykkeApiErrorCode(nameof(AdminNotFound), "The given admin is not found.");

            /// <summary>
            ///     The given admin is not active
            /// </summary>
            public static ILykkeApiErrorCode AdminNotActive =
                new LykkeApiErrorCode(nameof(AdminNotActive), "The given admin is not active.");

            /// <summary>
            ///     The new password is invalid
            /// </summary>
            public static ILykkeApiErrorCode NewPasswordInvalid =
                new LykkeApiErrorCode(nameof(NewPasswordInvalid), "The new password is invalid.");
            
            /// <summary>
            ///     The file is invalid
            /// </summary>
            public static ILykkeApiErrorCode InvalidFile =
                new LykkeApiErrorCode(nameof(InvalidFile), "The file is invalid.");
            
            /// <summary>
            ///     An error occurred while reading file
            /// </summary>
            public static ILykkeApiErrorCode CanNotReadFile =
                new LykkeApiErrorCode(nameof(CanNotReadFile), "An error occurred while reading file.");
        }

        /// <summary>
        ///     Group for all model validation error codes.
        /// </summary>
        public static class ModelValidation
        {
            /// <summary>
            ///     Common error code for any failed validation.
            ///     Use it as default validation error code if specific code is not required.
            /// </summary>
            public static readonly ILykkeApiErrorCode ModelValidationFailed =
                new LykkeApiErrorCode(nameof(ModelValidationFailed), "The model is invalid.");
        }
    }
}
