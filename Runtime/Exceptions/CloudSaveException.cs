using System;
using System.Collections.Generic;
using Unity.GameBackend.CloudSave.Models;
using Unity.Services.Core;
using UnityEngine.Scripting;

namespace Unity.Services.CloudSave
{
    [Preserve]
    public enum CloudSaveExceptionReason
    {
        Unknown,
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
    
    [Preserve]
    public class CloudSaveException: RequestFailedException
    {
        [Preserve] public CloudSaveExceptionReason Reason { get; private set; }
        
        internal CloudSaveException(CloudSaveExceptionReason reason, int errorCode, string message, Exception innerException)
            : base(errorCode, message, innerException)
        {
            Reason = reason;
        }
    }
    
    [Preserve]
    public class CloudSaveValidationException: CloudSaveException
    {
        [Preserve] public List<CloudSaveValidationErrorDetail> Details { get; private set; }

        internal CloudSaveValidationException(CloudSaveExceptionReason reason, int errorCode, string message, Exception innerException)
            : base(reason, errorCode, message, innerException)
        {
            Details = new List<CloudSaveValidationErrorDetail>();
        }

        internal CloudSaveValidationException(CloudSaveExceptionReason reason, int errorCode, string message,
            List<CloudSaveValidationErrorDetail> details, Exception innerException)
            : base(reason, errorCode, message, innerException)
        {
            Details = details;
        }
    }

    [Preserve]
    public class CloudSaveValidationErrorDetail
    {
        /// <summary>
        /// Single error in the Validation Error Response.
        /// </summary>
        /// <param name="field">field parameter</param>
        /// <param name="messages">messages parameter</param>
        /// <param name="key"></param>
        [Preserve]
        public CloudSaveValidationErrorDetail(string field, List<string> messages, string key = null)
        {
            Field = field;
            Messages = messages;
            Key = key;
        }

        internal CloudSaveValidationErrorDetail(ValidationErrorBody errorBody)
        {
            Field = errorBody.Field;
            Messages = errorBody.Messages;
        }
        
        internal CloudSaveValidationErrorDetail(BatchValidationErrorBody errorBody)
        {
            Field = errorBody.Field;
            Key = errorBody.Key;
            Messages = errorBody.Messages;
        }
        
        [Preserve]
        public string Field { get; }
        
        [Preserve]
        public string Key { get; }

        [Preserve]
        public List<string> Messages { get; }
    }
}
