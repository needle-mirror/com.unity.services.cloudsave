using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Unity.Services.CloudSave.Internal;
using Unity.Services.CloudSave.Internal.Http;
using Unity.Services.CloudSave.Internal.Models;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace Unity.Services.CloudSave
{
    public interface ICloudSaveDataClient
    {
        Task<List<string>> RetrieveAllKeysAsync();
        Task ForceSaveAsync(Dictionary<string, object> data);
        Task ForceDeleteAsync(string key);
        Task<Dictionary<string, string>> LoadAsync(HashSet<string> keys = null);
        Task<Dictionary<string, string>> LoadAllAsync();
    }

    internal class SaveDataInternal : ICloudSaveDataClient
    {
        readonly IApiClient _mApiClient;
        readonly ICloudSaveApiErrorHandler _mErrorHandler;

        internal SaveDataInternal(IApiClient apiClient, ICloudSaveApiErrorHandler errorHandler)
        {
            _mApiClient = apiClient;
            _mErrorHandler = errorHandler;
        }

        /// <summary>
        /// Returns all keys stored in Cloud Save for the logged in player.
        /// Throws a CloudSaveException with a reason code and explanation of what happened.
        ///
        /// This method includes pagination.
        /// </summary>
        /// <returns>A list of keys stored in the server for the logged in player.</returns>
        /// <exception cref="CloudSaveException">Thrown if request is unsuccessful.</exception>
        /// <exception cref="CloudSaveValidationException">Thrown if the service returned validation error.</exception>
        /// <exception cref="CloudSaveRateLimitedException">Thrown if the service returned rate limited error.</exception>
        public async Task<List<string>> RetrieveAllKeysAsync()
        {
            try
            {
                if (_mErrorHandler.IsRateLimited)
                {
                    throw _mErrorHandler.CreateFakeRateLimitException();
                }

                List<string> returnSet = new List<string>();
                Response<GetKeysResponse> response;
                string lastAddedKey = null;
                do
                {
                    response = await _mApiClient.RetrieveKeysAsync(lastAddedKey);
                    List<KeyMetadata> items = response.Result.Results;
                    if (items.Count > 0)
                    {
                        foreach (var item in items)
                        {
                            returnSet.Add(item.Key);
                        }

                        lastAddedKey = items[items.Count - 1].Key;
                    }
                }
                while (!string.IsNullOrEmpty(response.Result.Links.Next));

                return returnSet;
            }
            catch (HttpException<BasicErrorResponse> e)
            {
                throw _mErrorHandler.HandleBasicResponseException(e);
            }
            catch (HttpException<ValidationErrorResponse> e)
            {
                throw _mErrorHandler.HandleValidationResponseException(e);
            }
            catch (ResponseDeserializationException e)
            {
                throw _mErrorHandler.HandleDeserializationException(e);
            }
            catch (HttpException e)
            {
                throw _mErrorHandler.HandleHttpException(e);
            }
            catch (Exception e)
            {
                if (e is CloudSaveException)
                {
                    throw;
                }

                throw _mErrorHandler.HandleException(e);
            }
        }

        /// <summary>
        /// Upload one or more key-value pairs to the Cloud Save service, overwriting any values
        /// that are currently stored under the given keys.
        /// Throws a CloudSaveException with a reason code and explanation of what happened.
        ///
        /// <code>Dictionary</code> as a parameter ensures the uniqueness of given keys.
        /// There is no client validation in place, which means the API can be called regardless if data is incorrect and/or missing.
        /// </summary>
        /// <param name="data">The dictionary of keys and corresponding values to upload</param>
        /// <exception cref="CloudSaveException">Thrown if request is unsuccessful.</exception>
        /// <exception cref="CloudSaveValidationException">Thrown if the service returned validation error.</exception>
        /// <exception cref="CloudSaveRateLimitedException">Thrown if the service returned rate limited error.</exception>
        public async Task ForceSaveAsync(Dictionary<string, object> data)
        {
            try
            {
                if (data.Count > 0)
                {
                    if (_mErrorHandler.IsRateLimited)
                    {
                        throw _mErrorHandler.CreateFakeRateLimitException();
                    }
                    await _mApiClient.ForceSaveAsync(data);
                }
            }
            catch (HttpException<BasicErrorResponse> e)
            {
                throw _mErrorHandler.HandleBasicResponseException(e);
            }
            catch (HttpException<BatchValidationErrorResponse> e)
            {
                throw _mErrorHandler.HandleBatchValidationResponseException(e);
            }
            catch (ResponseDeserializationException e)
            {
                throw _mErrorHandler.HandleDeserializationException(e);
            }
            catch (HttpException e)
            {
                throw _mErrorHandler.HandleHttpException(e);
            }
            catch (Exception e)
            {
                if (e is CloudSaveException)
                {
                    throw;
                }

                throw _mErrorHandler.HandleException(e);
            }
        }

        /// <summary>
        /// Removes one key at the time. If a given key doesn't exist, there is no feedback in place to inform a developer about it.
        /// There is no client validation implemented for this method.
        /// Throws a CloudSaveException with a reason code and explanation of what happened.
        ///
        /// </summary>
        /// <param name="key">The key to be removed from the server</param>
        /// <exception cref="CloudSaveException">Thrown if request is unsuccessful.</exception>
        /// <exception cref="CloudSaveValidationException">Thrown if the service returned validation error.</exception>
        /// <exception cref="CloudSaveRateLimitedException">Thrown if the service returned rate limited error.</exception>
        public async Task ForceDeleteAsync(string key)
        {
            try
            {
                if (_mErrorHandler.IsRateLimited)
                {
                    throw _mErrorHandler.CreateFakeRateLimitException();
                }
                await _mApiClient.ForceDeleteAsync(key);
            }
            catch (HttpException<BasicErrorResponse> e)
            {
                throw _mErrorHandler.HandleBasicResponseException(e);
            }
            catch (HttpException<ValidationErrorResponse> e)
            {
                throw _mErrorHandler.HandleValidationResponseException(e);
            }
            catch (ResponseDeserializationException e)
            {
                throw _mErrorHandler.HandleDeserializationException(e);
            }
            catch (HttpException e)
            {
                throw _mErrorHandler.HandleHttpException(e);
            }
            catch (Exception e)
            {
                if (e is CloudSaveException)
                {
                    throw;
                }

                throw _mErrorHandler.HandleException(e);
            }
        }

        /// <summary>
        /// Downloads one or more values from Cloud Save, based on provided keys.
        /// <code>HashSet</code> as a parameter ensures the uniqueness of keys.
        /// There is no client validation in place.
        /// This method includes pagination.
        /// Throws a CloudSaveException with a reason code and explanation of what happened.
        /// </summary>
        /// <param name="keys">The HashSet of keys to download from the server</param>
        /// <returns>The dictionary of key-value pairs that represents the current state of data on the server</returns>
        /// <exception cref="CloudSaveException">Thrown if request is unsuccessful.</exception>
        /// <exception cref="CloudSaveValidationException">Thrown if the service returned validation error.</exception>
        /// <exception cref="CloudSaveRateLimitedException">Thrown if the service returned rate limited error.</exception>
        public async Task<Dictionary<string, string>> LoadAsync(HashSet<string> keys = null)
        {
            var result = new Dictionary<string, string>();
            try
            {
                if (_mErrorHandler.IsRateLimited)
                {
                    throw _mErrorHandler.CreateFakeRateLimitException();
                }

                Response<GetItemsResponse> response;
                string lastAddedKey = null;
                do
                {
                    response = await _mApiClient.LoadAsync(keys, lastAddedKey);
                    List<Item> items = response.Result.Results;
                    if (items.Count > 0)
                    {
                        foreach (var item in items)
                        {
                            result[item.Key] = item.Value.GetAsString();
                        }

                        lastAddedKey = items[items.Count - 1].Key;
                    }
                }
                while (!string.IsNullOrEmpty(response.Result.Links.Next));

                return result;
            }
            catch (HttpException<BasicErrorResponse> e)
            {
                throw _mErrorHandler.HandleBasicResponseException(e);
            }
            catch (HttpException<ValidationErrorResponse> e)
            {
                throw _mErrorHandler.HandleValidationResponseException(e);
            }
            catch (ResponseDeserializationException e)
            {
                throw _mErrorHandler.HandleDeserializationException(e);
            }
            catch (HttpException e)
            {
                throw _mErrorHandler.HandleHttpException(e);
            }
            catch (Exception e)
            {
                if (e is CloudSaveException)
                {
                    throw;
                }

                throw _mErrorHandler.HandleException(e);
            }
        }

        /// <summary>
        /// Downloads all data from Cloud Save.
        /// There is no client validation in place.
        /// This method includes pagination.
        /// Throws a CloudSaveException with a reason code and explanation of what happened.
        /// </summary>
        /// <returns>The dictionary of all key-value pairs that represents the current state of data on the server</returns>
        /// <exception cref="CloudSaveException">Thrown if request is unsuccessful.</exception>
        /// <exception cref="CloudSaveValidationException">Thrown if the service returned validation error.</exception>
        /// <exception cref="CloudSaveRateLimitedException">Thrown if the service returned rate limited error.</exception>
        public async Task<Dictionary<string, string>> LoadAllAsync()
        {
            return await LoadAsync();
        }
    }
}
