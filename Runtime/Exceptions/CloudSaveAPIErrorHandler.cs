using System;
using Unity.Services.CloudSave.Internal.Http;
using Unity.Services.CloudSave.Internal.Models;
using Unity.Services.Core;

namespace Unity.Services.CloudSave
{
    internal interface ICloudSaveApiErrorHandler
    {
        CloudSaveException HandleBasicResponseException(HttpException<BasicErrorResponse> response);
        CloudSaveValidationException HandleValidationResponseException(HttpException<ValidationErrorResponse> response);
        CloudSaveValidationException HandleBatchValidationResponseException(HttpException<BatchValidationErrorResponse> response);
        CloudSaveException HandleDeserializationException(ResponseDeserializationException exception);
        CloudSaveException HandleHttpException(HttpException exception);
        CloudSaveException HandleException(Exception exception);
    }

    internal class CloudSaveApiErrorHandler : ICloudSaveApiErrorHandler
    {
        public CloudSaveException HandleHttpException(HttpException exception)
        {
            if (exception.Response.IsNetworkError)
            {
                return new CloudSaveException(CloudSaveExceptionReason.NoInternetConnection, CommonErrorCodes.TransportError, 
                    "The request to the Cloud Save service failed - make sure you're connected to an internet connection and try again.", 
                    exception.InnerException);
            }
            
            return new CloudSaveException(GetReason(exception.Response.StatusCode), CommonErrorCodes.Unknown, 
                exception.Response.ErrorMessage ?? GetGenericMessage(exception.Response.StatusCode), exception.InnerException);
        }

        public CloudSaveException HandleException(Exception exception)
        {
            return new CloudSaveException(CloudSaveExceptionReason.Unknown, CommonErrorCodes.Unknown, 
                "An unknown error occurred in the Cloud Save SDK.", exception);
        }

        public CloudSaveException HandleDeserializationException(ResponseDeserializationException exception)
        {
            return new CloudSaveException(GetReason(exception.response.StatusCode), CommonErrorCodes.Unknown, 
                exception.response.ErrorMessage ?? GetGenericMessage(exception.response.StatusCode), exception.InnerException);

        }

        public CloudSaveException HandleBasicResponseException(HttpException<BasicErrorResponse> response)
        {
            var message = String.IsNullOrEmpty(response.ActualError.Detail) 
                ? GetGenericMessage(response.Response.StatusCode) : response.ActualError.Detail;
            
            return new CloudSaveException(GetReason(response.Response.StatusCode), response.ActualError.Code, message, 
                response.InnerException);
        }
        
        public CloudSaveValidationException HandleValidationResponseException(HttpException<ValidationErrorResponse> response)
        {
            var message = "There was a validation error. Check 'Details' for more information.";
            
            CloudSaveValidationException exception = new CloudSaveValidationException(GetReason(response.Response.StatusCode), 
                response.ActualError.Code, message, response.InnerException);

            foreach (var error in response.ActualError.Errors)
            {
                exception.Details.Add(new CloudSaveValidationErrorDetail(error));
            }

            return exception;
        }
        
        public CloudSaveValidationException HandleBatchValidationResponseException(HttpException<BatchValidationErrorResponse> response)
        {
            var message = "There was a validation error. Check 'Details' for more information.";
            
            CloudSaveValidationException exception = new CloudSaveValidationException(GetReason(response.Response.StatusCode), 
                response.ActualError.Code, message, response.InnerException);

            foreach (var error in response.ActualError.Errors)
            {
                exception.Details.Add(new CloudSaveValidationErrorDetail(error));
            }

            return exception;
        }

        CloudSaveExceptionReason GetReason(long statusCode)
        {
            switch (statusCode)
            {
                case 400:
                    return CloudSaveExceptionReason.InvalidArgument;
                case 401:
                    return CloudSaveExceptionReason.Unauthorized;
                case 403:
                    return CloudSaveExceptionReason.KeyLimitExceeded;
                case 404:
                    return CloudSaveExceptionReason.NotFound;
                case 429:
                    return CloudSaveExceptionReason.TooManyRequests;
                case 500:
                case 503:
                    return CloudSaveExceptionReason.ServiceUnavailable;
                default:
                    return CloudSaveExceptionReason.Unknown;
            }
        }
        
        string GetGenericMessage(long statusCode)
        {
            switch (statusCode)
            {
                case 400:
                    return "Some of the arguments passed to the Cloud Save request were invalid. Please check the requirements and try again.";
                case 401:
                    return "Permission denied when making a request to the Cloud Save service. Ensure you are signed in through the Authentication SDK and try again.";
                case 403:
                    return "Key-value pair limit per user exceeded.";
                case 404:
                    return "The requested action could not be completed as the specified resource is not found - please make sure it exists, then try again.";
                case 429:
                    return "Too many requests have been sent, so this device has been rate limited. Please try again later.";
                case 500:
                case 503:
                    return "Cloud Save service is currently unavailable. Please try again later.";
                default:
                    return "An unknown error occurred in the Cloud Save SDK.";
            }
        }
    }
}
