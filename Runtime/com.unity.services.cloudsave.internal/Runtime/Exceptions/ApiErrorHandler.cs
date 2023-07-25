using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Unity.Services.CloudSave.Internal.Http;
using Unity.Services.CloudSave.Internal.Models;
using Unity.Services.Core;

[assembly: InternalsVisibleTo("Unity.Services.CloudSave.Tests")]

namespace Unity.Services.CloudSave.Internal
{
    interface IApiErrorHandler
    {
        Task<T> RunWithErrorHandling<T>(Func<Task<T>> func);
        Task RunWithErrorHandling(Func<Task> func);
    }

    class ApiErrorHandler : IApiErrorHandler
    {
        readonly IRateLimiter _rateLimiter;
        CloudSaveRateLimitedException _exception;

        public ApiErrorHandler(IRateLimiter rateLimiter)
        {
            _rateLimiter = rateLimiter;
        }

        Exception ResolveException(Exception exception)
        {
            return exception switch
            {
                HttpException<BasicErrorResponse> e => HandleBasicResponseException(e),
                HttpException<BatchValidationErrorResponse> e => HandleBatchValidationResponseException(e),
                HttpException<ValidationErrorResponse> e => HandleValidationResponseException(e),
                HttpException<GCSErrorResponse> e => HandleGCSResponseException(e),
                ResponseDeserializationException e => HandleDeserializationException(e),
                HttpException<BatchConflictErrorResponse> e => HandleBatchConflictResponseException(e),
                HttpException<DeleteConflictErrorResponse> e => HandleDeleteConflictResponseException(e),
                HttpException e => HandleHttpException(e),
                CloudSaveException e => e,
                _ => HandleException(exception)
            };
        }

        public async Task<T> RunWithErrorHandling<T>(Func<Task<T>> func)
        {
            if (IsRateLimited)
            {
                throw CreateRateLimitException();
            }

            try
            {
                return await func();
            }
            catch (Exception e)
            {
                throw ResolveException(e);
            }
        }

        public async Task RunWithErrorHandling(Func<Task> func)
        {
            if (IsRateLimited)
            {
                throw CreateRateLimitException();
            }

            try
            {
                await func();
            }
            catch (Exception e)
            {
                throw ResolveException(e);
            }
        }

        internal bool IsRateLimited => _rateLimiter.RateLimited;

        internal CloudSaveRateLimitedException CreateRateLimitException()
        {
            if (_exception == null)
            {
                var error = new BasicErrorResponse("TooManyRequests", status: 429);
                var response = new HttpClientResponse(new Dictionary<string, string>(), 429,
                    true, false, Array.Empty<byte>(), string.Empty);

                _exception = new CloudSaveRateLimitedException(GetReason(429), 429, GetGenericMessage(429),
                    _rateLimiter.RetryAfter, new HttpException<BasicErrorResponse>(response, error));
            }

            _exception.RetryAfter = _rateLimiter.RetryAfter;
            return _exception;
        }

        internal CloudSaveException HandleBasicResponseException(HttpException<BasicErrorResponse> response)
        {
            var message = string.IsNullOrEmpty(response.ActualError.Detail)
                ? GetGenericMessage(response.Response.StatusCode)
                : response.ActualError.Detail;

            if (_rateLimiter.IsRateLimitException(response))
            {
                _rateLimiter.ProcessRateLimit(response);
                _exception = new CloudSaveRateLimitedException(GetReason(response.Response.StatusCode),
                    response.ActualError.Code,
                    message, _rateLimiter.RetryAfter, response);

                return _exception;
            }

            return new CloudSaveException(GetReason(response.Response.StatusCode), response.ActualError.Code, message,
                response);
        }

        internal CloudSaveException HandleHttpException(HttpException exception)
        {
            if (exception.Response.IsNetworkError)
            {
                const string requestFailedMessage =
                    "The request to the Cloud Save service failed - make sure you're connected to an internet connection and try again.";
                return new CloudSaveException(CloudSaveExceptionReason.NoInternetConnection,
                    CommonErrorCodes.TransportError, requestFailedMessage, exception);
            }

            var message = exception.Response.ErrorMessage ?? GetGenericMessage(exception.Response.StatusCode);
            return new CloudSaveException(GetReason(exception.Response.StatusCode), CommonErrorCodes.Unknown, message,
                exception);
        }

        internal CloudSaveException HandleException(Exception exception)
        {
            const string message = "An unknown error occurred in the Cloud Save SDK.";

            return new CloudSaveException(CloudSaveExceptionReason.Unknown, CommonErrorCodes.Unknown, message,
                exception);
        }

        internal CloudSaveException HandleDeserializationException(ResponseDeserializationException exception)
        {
            var message = exception.response.ErrorMessage ?? GetGenericMessage(exception.response.StatusCode);

            return new CloudSaveException(GetReason(exception.response.StatusCode), CommonErrorCodes.Unknown, message,
                exception);
        }

        internal CloudSaveException HandleGCSResponseException(HttpException<GCSErrorResponse> response)
        {
            var message = GetGenericMessage(response.Response.StatusCode);

            return new CloudSaveException(GetReason(response.Response.StatusCode),
                GetGCSCode(response.Response.StatusCode), message,
                response);
        }

        internal CloudSaveConflictException HandleDeleteConflictResponseException(
            HttpException<DeleteConflictErrorResponse> response)
        {
            var message = string.IsNullOrEmpty(response.ActualError.Detail)
                ? GetGenericMessage(response.Response.StatusCode)
                : response.ActualError.Detail;
            var data = response.ActualError.Data;
            return new CloudSaveConflictException(GetReason(response.Response.StatusCode), response.ActualError.Code,
                message,
                new List<CloudSaveConflictErrorDetail>
                {
                    new CloudSaveConflictErrorDetail(data.Key, data.AttemptedWriteLock, data.ExistingWriteLock)
                },
                response);
        }

        internal CloudSaveConflictException HandleBatchConflictResponseException(
            HttpException<BatchConflictErrorResponse> response)
        {
            var message = string.IsNullOrEmpty(response.ActualError.Detail)
                ? GetGenericMessage(response.Response.StatusCode)
                : response.ActualError.Detail;

            var details = new List<CloudSaveConflictErrorDetail>();
            foreach (var error in response.ActualError.Data)
            {
                details.Add(new CloudSaveConflictErrorDetail(error.Attempted.Key, error.Attempted.WriteLock,
                    error.Existing.WriteLock));
            }

            return new CloudSaveConflictException(GetReason(response.Response.StatusCode), response.ActualError.Code,
                message, details,
                response);
        }

        internal CloudSaveValidationException HandleValidationResponseException(
            HttpException<ValidationErrorResponse> response)
        {
            const string message = "There was a validation error. Check 'Details' for more information.";

            var exception = new CloudSaveValidationException(GetReason(response.Response.StatusCode),
                response.ActualError.Code, message, response);

            foreach (var error in response.ActualError.Errors)
            {
                exception.Details.Add(new CloudSaveValidationErrorDetail(error));
            }

            return exception;
        }

        internal CloudSaveValidationException HandleBatchValidationResponseException(
            HttpException<BatchValidationErrorResponse> response)
        {
            const string message = "There was a validation error. Check 'Details' for more information.";

            var exception = new CloudSaveValidationException(GetReason(response.Response.StatusCode),
                response.ActualError.Code, message, response);

            foreach (var error in response.ActualError.Errors)
            {
                exception.Details.Add(new CloudSaveValidationErrorDetail(error));
            }

            return exception;
        }

        static CloudSaveExceptionReason GetReason(long statusCode)
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
                case 409:
                    return CloudSaveExceptionReason.Conflict;
                // GCS returns a 412 when a writeLock does not match, but we treat it as a conflict
                case 412:
                    return CloudSaveExceptionReason.Conflict;
                case 429:
                    return CloudSaveExceptionReason.TooManyRequests;
                case 500:
                case 503:
                    return CloudSaveExceptionReason.ServiceUnavailable;
                default:
                    return CloudSaveExceptionReason.Unknown;
            }
        }

        static string GetGenericMessage(long statusCode)
        {
            switch (statusCode)
            {
                case 400:
                    return
                        "Some of the arguments passed to the Cloud Save request were invalid. Please check the requirements and try again.";
                case 401:
                    return
                        "Permission denied when making a request to the Cloud Save service. Ensure you are signed in through the Authentication SDK and try again.";
                case 403:
                    return "Key-value pair limit per user exceeded.";
                case 404:
                    return
                        "The requested action could not be completed as the specified resource is not found - please make sure it exists, then try again.";
                case 409:
                    return "WriteLock in one or more data items within request does not match stored WriteLock.";
                case 412:
                    return "WriteLock in file upload request does not match stored WriteLock.";
                case 429:
                    return
                        "Too many requests have been sent, so this device has been rate limited. Please try again later.";
                case 500:
                case 503:
                    return "Cloud Save service is currently unavailable. Please try again later.";
                default:
                    return "An unknown error occurred in the Cloud Save SDK.";
            }
        }

        static int GetGCSCode(long statusCode)
        {
            return statusCode switch
            {
                404 => 7007,
                412 => 7012,
                _ => 1000
            };
        }
    }
}
