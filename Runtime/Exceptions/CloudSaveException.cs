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
        Unknown = 0,
        NoInternetConnection,
        ProjectIdMissing,
        PlayerIdMissing,
        AccessTokenMissing,
        InvalidArgument,
        Unauthorized,
        KeyLimitExceeded,
        NotFound,
        TooManyRequests,
        ServiceUnavailable
    }

    /// <summary>
    /// Represents a generic error.
    /// </summary>
    [Preserve]
    public class CloudSaveException : RequestFailedException
    {
        [Preserve] public CloudSaveExceptionReason Reason { get; private set; }

        internal CloudSaveException(CloudSaveExceptionReason reason, int errorCode, string message, Exception innerException)
            : base(errorCode, message, innerException)
        {
            Reason = reason;
        }
    }
}
