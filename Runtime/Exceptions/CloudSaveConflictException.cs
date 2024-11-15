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
        /// <summary>
        /// The details of the exception
        /// </summary>
        [Preserve] public List<CloudSaveConflictErrorDetail> Details { get; private set; }

        internal CloudSaveConflictException(CloudSaveExceptionReason reason, int errorCode, string message,
                                            List<CloudSaveConflictErrorDetail> details, Exception innerException)
            : base(reason, errorCode, message, innerException)
        {
            Details = details;
        }
    }

    /// <summary>
    /// Represents the details for a conflict error from the Cloud Save service.
    /// </summary>
    [Preserve]
    public class CloudSaveConflictErrorDetail
    {
        /// <summary>
        /// Single error in the Validation Error Response.
        /// </summary>
        /// <param name="key">The item key.</param>
        /// <param name="attemptedWriteLock">The conflict write lock in the data that caused the error.</param>
        /// <param name="existingWriteLock">The current existing write lock.</param>
        internal CloudSaveConflictErrorDetail(string key, string attemptedWriteLock, string existingWriteLock)
        {
            Key = key;
            AttemptedWriteLock = attemptedWriteLock;
            ExistingWriteLock = existingWriteLock;
        }

        /// <summary>
        /// The item key.
        /// </summary>
        [Preserve]
        public string Key { get; }

        /// <summary>
        /// The conflict write lock in the data that caused the error.
        /// </summary>
        [Preserve]
        public string AttemptedWriteLock { get; }

        /// <summary>
        /// The current existing write locks.
        /// </summary>
        [Preserve]
        public string ExistingWriteLock { get; }
    }
}
