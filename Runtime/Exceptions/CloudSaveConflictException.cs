using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace Unity.Services.CloudSave
{
    /// <summary>
    /// Represents a conflict error from the Cloud Save service.
    /// </summary>
    [Preserve]
    public class CloudSaveConflictException : CloudSaveException
    {
        [Preserve] public List<CloudSaveConflictErrorDetail> Details { get; private set; }

        internal CloudSaveConflictException(CloudSaveExceptionReason reason, int errorCode, string message,
                                            List<CloudSaveConflictErrorDetail> details, Exception innerException)
            : base(reason, errorCode, message, innerException)
        {
            Details = details;
        }
    }

    [Preserve]
    public class CloudSaveConflictErrorDetail
    {
        /// <summary>
        /// Single error in the Validation Error Response.
        /// </summary>
        /// <param name="key">The item key.</param>
        /// <param name="attemptedWriteLock">The conflict write lock in the data that caused the error.</param>
        /// <param name="existingWriteLock">The current existing write lock</param>

        internal CloudSaveConflictErrorDetail(string key, string attemptedWriteLock, string existingWriteLock)
        {
            Key = key;
            AttemptedWriteLock = attemptedWriteLock;
            ExistingWriteLock = existingWriteLock;
        }

        [Preserve]
        public string Key { get; }

        [Preserve]
        public string AttemptedWriteLock { get; }

        [Preserve]
        public string ExistingWriteLock { get; }
    }
}
