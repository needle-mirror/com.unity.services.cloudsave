using System;
using Unity.Services.Core;
using UnityEngine.Scripting;

namespace Unity.Services.CloudSave
{
    /// <summary>
    /// An enum of possible reasons that Cloud Save would throw an exception.
    /// </summary>
    [Preserve]
    public enum CloudSaveExceptionReason
    {
        /// <summary>
        /// Enum value for unknown reason.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Enum value for no internet connection.
        /// </summary>
        NoInternetConnection,

        /// <summary>
        /// Enum value for a missing project ID.
        /// </summary>
        ProjectIdMissing,

        /// <summary>
        /// Enum value for a missing playerx ID.
        /// </summary>
        PlayerIdMissing,

        /// <summary>
        /// Enum value for a missing access token.
        /// </summary>
        AccessTokenMissing,

        /// <summary>
        /// Enum value for an invalid argument.
        /// </summary>
        InvalidArgument,

        /// <summary>
        /// Enum value for an unauthorized request.
        /// </summary>
        Unauthorized,

        /// <summary>
        /// Enum value for a request with too many keys.
        /// </summary>
        KeyLimitExceeded,

        /// <summary>
        /// Enum value for a not found result.
        /// </summary>
        NotFound,

        /// <summary>
        /// Enum value for max requests exceeded.
        /// </summary>
        TooManyRequests,

        /// <summary>
        /// Enum value for the service being unavailable.
        /// </summary>
        ServiceUnavailable,

        /// <summary>
        /// Enum value for a conflict.
        /// </summary>
        Conflict
    }

    /// <summary>
    /// Represents a generic error.
    /// </summary>
    [Preserve]
    public class CloudSaveException : RequestFailedException
    {
        /// <summary>
        /// The reason for the exception
        /// </summary>
        [Preserve] public CloudSaveExceptionReason Reason { get; private set; }

        internal CloudSaveException(CloudSaveExceptionReason reason, int errorCode, string message, Exception innerException)
            : base(errorCode, message, innerException)
        {
            Reason = reason;
        }
    }
}
