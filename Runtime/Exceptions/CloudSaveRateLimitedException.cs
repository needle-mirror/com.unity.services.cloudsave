using System;

namespace Unity.Services.CloudSave
{
    /// <summary>
    /// An exception that is thrown when the client has been rate limited.
    /// </summary>
    public class CloudSaveRateLimitedException : CloudSaveException
    {
        /// <summary>
        /// The number of seconds until the client is no longer rate limited.
        /// </summary>
        public float RetryAfter { get; set; }

        internal CloudSaveRateLimitedException(CloudSaveExceptionReason reason, int errorCode, string message,
                                               float retryAfter, Exception innerException) : base(reason, errorCode, message, innerException)
        {
            RetryAfter = retryAfter;
        }
    }
}
