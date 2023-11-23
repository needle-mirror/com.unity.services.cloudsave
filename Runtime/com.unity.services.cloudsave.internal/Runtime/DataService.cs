using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.CloudSave.Internal.Http;
using Unity.Services.CloudSave.Internal.Models;

namespace Unity.Services.CloudSave.Internal
{
    public interface IDataService
    {
        public IPlayerDataService Player { get; }
        public ICustomDataService Custom { get; }

        #region Deprecated, pre v3.0.0
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
        [Obsolete("This method will be removed in an upcoming release. In order to retrieve keys, call: Task<List<ItemKey>> CloudSaveService.Instance.Data.Player.ListAllKeysAsync() and use ItemKey.Key to get the string value for each key", false)]
        Task<List<string>> RetrieveAllKeysAsync();

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
        [Obsolete("This method will be removed in an upcoming release. In order to save data without write lock validation, call: Task CloudSaveService.Instance.Data.Player.SaveAsync(Dictionary<string, object> data)", false)]
        Task ForceSaveAsync(Dictionary<string, object> data);

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
        [Obsolete("This method will be removed in an upcoming release. In order to delete a key without write lock validation, call: Task CloudSaveService.Instance.Data.Player.DeleteAsync(string key)", false)]
        Task ForceDeleteAsync(string key);

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
        [Obsolete("This method will be removed in an upcoming release. In order to get a serialized object value, call: Task<Dictionary<string, Item>> CloudSaveService.Instance.Data.Player.LoadAsync(ISet<string> keys) and use the GetAsString() method provided by IDeserializable on Item.Value", false)]
        Task<Dictionary<string, string>> LoadAsync(HashSet<string> keys = null);

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
        [Obsolete("This method will be removed in an upcoming release. In order to get all serialized object values, call: Task<Dictionary<string, Item>> CloudSaveService.Instance.Data.Player.LoadAllAsync() and use the GetAsString() method provided by IDeserializable on Item.Value", false)]
        Task<Dictionary<string, string>> LoadAllAsync();
        #endregion
    }

    class DataService : IDataService
    {
        readonly IPlayerDataApiClient m_playerDataApiClient;
        readonly ICloudSaveApiErrorHandlerDepr m_ErrorHandlerDepr;

        internal DataService(IPlayerDataApiClient playerDataClient, ICloudSaveApiErrorHandlerDepr errorHandlerDepr, IPlayerDataService playerDataService, ICustomDataService customDataService)
        {
            Player = playerDataService;
            Custom = customDataService;

            // API client and error handlers for deprecated endpoints, pre v3.0.0
            m_playerDataApiClient = playerDataClient;
            m_ErrorHandlerDepr = errorHandlerDepr;
        }

        public IPlayerDataService Player { get; }
        public ICustomDataService Custom { get; }

        internal static FieldFilter FieldFilterToInternalFieldFilter(CloudSave.Models.FieldFilter fieldFilter)
        {
            return new FieldFilter(fieldFilter.Key, fieldFilter.Value, (FieldFilter.OpOptions)fieldFilter.Op,
                fieldFilter.Asc);
        }

        #region Deprecated, pre v3.0.0
        public async Task<List<string>> RetrieveAllKeysAsync()
        {
            try
            {
                if (m_ErrorHandlerDepr.IsRateLimited)
                {
                    throw m_ErrorHandlerDepr.CreateFakeRateLimitException();
                }

                var returnSet = new List<string>();
                Response<GetKeysResponse> response;
                string lastAddedKey = null;
                do
                {
                    response = await m_playerDataApiClient.ListKeysAsync(lastAddedKey);
                    var items = response.Result.Results;
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
                throw m_ErrorHandlerDepr.HandleBasicResponseException(e);
            }
            catch (HttpException<ValidationErrorResponse> e)
            {
                throw m_ErrorHandlerDepr.HandleValidationResponseException(e);
            }
            catch (ResponseDeserializationException e)
            {
                throw m_ErrorHandlerDepr.HandleDeserializationException(e);
            }
            catch (HttpException e)
            {
                throw m_ErrorHandlerDepr.HandleHttpException(e);
            }
            catch (Exception e)
            {
                if (e is CloudSaveException)
                {
                    throw;
                }

                throw m_ErrorHandlerDepr.HandleException(e);
            }
        }

        public async Task ForceSaveAsync(Dictionary<string, object> data)
        {
            try
            {
                if (data.Count > 0)
                {
                    if (m_ErrorHandlerDepr.IsRateLimited)
                    {
                        throw m_ErrorHandlerDepr.CreateFakeRateLimitException();
                    }

                    await m_playerDataApiClient.ForceSaveAsync(data);
                }
            }
            catch (HttpException<BasicErrorResponse> e)
            {
                throw m_ErrorHandlerDepr.HandleBasicResponseException(e);
            }
            catch (HttpException<BatchValidationErrorResponse> e)
            {
                throw m_ErrorHandlerDepr.HandleBatchValidationResponseException(e);
            }
            catch (ResponseDeserializationException e)
            {
                throw m_ErrorHandlerDepr.HandleDeserializationException(e);
            }
            catch (HttpException e)
            {
                throw m_ErrorHandlerDepr.HandleHttpException(e);
            }
            catch (Exception e)
            {
                if (e is CloudSaveException)
                {
                    throw;
                }

                throw m_ErrorHandlerDepr.HandleException(e);
            }
        }

        public async Task ForceDeleteAsync(string key)
        {
            try
            {
                if (m_ErrorHandlerDepr.IsRateLimited)
                {
                    throw m_ErrorHandlerDepr.CreateFakeRateLimitException();
                }
                await m_playerDataApiClient.DeleteAsync(key);
            }
            catch (HttpException<BasicErrorResponse> e)
            {
                throw m_ErrorHandlerDepr.HandleBasicResponseException(e);
            }
            catch (HttpException<ValidationErrorResponse> e)
            {
                throw m_ErrorHandlerDepr.HandleValidationResponseException(e);
            }
            catch (ResponseDeserializationException e)
            {
                throw m_ErrorHandlerDepr.HandleDeserializationException(e);
            }
            catch (HttpException e)
            {
                throw m_ErrorHandlerDepr.HandleHttpException(e);
            }
            catch (Exception e)
            {
                if (e is CloudSaveException)
                {
                    throw;
                }

                throw m_ErrorHandlerDepr.HandleException(e);
            }
        }

        public async Task<Dictionary<string, string>> LoadAsync(HashSet<string> keys = null)
        {
            var result = new Dictionary<string, string>();
            try
            {
                if (m_ErrorHandlerDepr.IsRateLimited)
                {
                    throw m_ErrorHandlerDepr.CreateFakeRateLimitException();
                }

                Response<GetItemsResponse> response;
                string lastAddedKey = null;
                do
                {
                    response = await m_playerDataApiClient.LoadAsync(keys, lastAddedKey);
                    var items = response.Result.Results;
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
                throw m_ErrorHandlerDepr.HandleBasicResponseException(e);
            }
            catch (HttpException<ValidationErrorResponse> e)
            {
                throw m_ErrorHandlerDepr.HandleValidationResponseException(e);
            }
            catch (ResponseDeserializationException e)
            {
                throw m_ErrorHandlerDepr.HandleDeserializationException(e);
            }
            catch (HttpException e)
            {
                throw m_ErrorHandlerDepr.HandleHttpException(e);
            }
            catch (Exception e)
            {
                if (e is CloudSaveException)
                {
                    throw;
                }

                throw m_ErrorHandlerDepr.HandleException(e);
            }
        }

        public async Task<Dictionary<string, string>> LoadAllAsync()
        {
            return await LoadAsync();
        }

        #endregion
    }
}
