using System;
using System.Collections.Generic;
using Unity.Services.CloudSave.Internal.Http;
using Unity.Services.CloudSave.Internal.Models;
using Unity.Services.Core;
using UnityEngine;

namespace Unity.Services.CloudSave
{
    internal interface ICloudSaveApiErrorHandler
    {
        bool IsRateLimited { get; }
        CloudSaveException HandleBasicResponseException(HttpException<BasicErrorResponse> response);
        CloudSaveValidationException HandleValidationResponseException(HttpException<ValidationErrorResponse> response);
        CloudSaveValidationException HandleBatchValidationResponseException(HttpException<BatchValidationErrorResponse> response);
        CloudSaveException HandleDeserializationException(ResponseDeserializationException exception);
        CloudSaveException HandleHttpException(HttpException exception);
        CloudSaveException HandleException(Exception exception);
        CloudSaveRateLimitedException CreateFakeRateLimitException();
    }

    internal class CloudSaveApiErrorHandler : ICloudSaveApiErrorHandler
    {
        readonly IRateLimiter _rateLimiter;
        CloudSaveRateLimitedException _exception;

        public CloudSaveApiErrorHandler(IRateLimiter rateLimiter)
        {
            _rateLimiter = rateLimiter;
        }

        public bool IsRateLimited => _rateLimiter.RateLimited;

        public CloudSaveRateLimitedException CreateFakeRateLimitException()
        {
            if (_exception == null)
            {
                BasicErrorResponse error = new BasicErrorResponse("TooManyRequests", status: 429);
                HttpClientResponse response = new HttpClientResponse(new Dictionary<string, string>(), 429,
                    true, false, new byte[0], String.Empty);

                _exception = new CloudSaveRateLimitedException(GetReason(429), 429, GetGenericMessage(429),
                    _rateLimiter.RetryAfter, new HttpException<BasicErrorResponse>(response, error));
            }

            _exception.RetryAfter = _rateLimiter.RetryAfter;
            PerformTemporaryErrorLogWorkaround("CloudSaveRateLimitedException", _exception.Message);
            return _exception;
        }

        public CloudSaveException HandleHttpException(HttpException exception)
        {
            if (exception.Response.IsNetworkError)
            {
                string requestFailedMessage = "The request to the Cloud Save service failed - make sure you're connected to an internet connection and try again.";
                PerformTemporaryErrorLogWorkaround("CloudSaveException", requestFailedMessage);
                return new CloudSaveException(CloudSaveExceptionReason.NoInternetConnection, CommonErrorCodes.TransportError, requestFailedMessage, exception);
            }

            string message = exception.Response.ErrorMessage ?? GetGenericMessage(exception.Response.StatusCode);
            PerformTemporaryErrorLogWorkaround("CloudSaveException", message);
            return new CloudSaveException(GetReason(exception.Response.StatusCode), CommonErrorCodes.Unknown, message, exception);
        }

        public CloudSaveException HandleException(Exception exception)
        {
            string message = "An unknown error occurred in the Cloud Save SDK.";

            PerformTemporaryErrorLogWorkaround("CloudSaveException", message);

            return new CloudSaveException(CloudSaveExceptionReason.Unknown, CommonErrorCodes.Unknown, message, exception);
        }

        public CloudSaveException HandleDeserializationException(ResponseDeserializationException exception)
        {
            string message = exception.response.ErrorMessage ?? GetGenericMessage(exception.response.StatusCode);

            PerformTemporaryErrorLogWorkaround("CloudSaveException", message);

            return new CloudSaveException(GetReason(exception.response.StatusCode), CommonErrorCodes.Unknown, message, exception);
        }

        public CloudSaveException HandleBasicResponseException(HttpException<BasicErrorResponse> response)
        {
            var message = String.IsNullOrEmpty(response.ActualError.Detail)
                ? GetGenericMessage(response.Response.StatusCode) : response.ActualError.Detail;

            if (_rateLimiter.IsRateLimitException(response))
            {
                _rateLimiter.ProcessRateLimit(response);
                _exception = new CloudSaveRateLimitedException(GetReason(response.Response.StatusCode),
                    response.ActualError.Code,
                    message, _rateLimiter.RetryAfter, response);
                PerformTemporaryErrorLogWorkaround("CloudSaveRateLimitedException", message);

                return _exception;
            }
            PerformTemporaryErrorLogWorkaround("CloudSaveException", message);

            return new CloudSaveException(GetReason(response.Response.StatusCode), response.ActualError.Code, message,
                response);
        }

        public CloudSaveValidationException HandleValidationResponseException(HttpException<ValidationErrorResponse> response)
        {
            var message = "There was a validation error. Check 'Details' for more information.";

            CloudSaveValidationException exception = new CloudSaveValidationException(GetReason(response.Response.StatusCode),
                response.ActualError.Code, message, response);

            foreach (var error in response.ActualError.Errors)
            {
                exception.Details.Add(new CloudSaveValidationErrorDetail(error));
            }

            PerformTemporaryErrorLogWorkaround("CloudSaveValidationException", message);

            return exception;
        }

        public CloudSaveValidationException HandleBatchValidationResponseException(HttpException<BatchValidationErrorResponse> response)
        {
            var message = "There was a validation error. Check 'Details' for more information.";

            CloudSaveValidationException exception = new CloudSaveValidationException(GetReason(response.Response.StatusCode),
                response.ActualError.Code, message, response);

            foreach (var error in response.ActualError.Errors)
            {
                exception.Details.Add(new CloudSaveValidationErrorDetail(error));
            }

            PerformTemporaryErrorLogWorkaround("CloudSaveValidationException", message);

            return exception;
        }

        internal void PerformTemporaryErrorLogWorkaround(string exceptionType, string message)
        {
            Debug.LogError($"{exceptionType}: {message}");
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
