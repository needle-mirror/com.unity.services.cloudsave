using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Unity.GameBackend.CloudSave;
using Unity.GameBackend.CloudSave.Http;
using Unity.GameBackend.CloudSave.Models;
using Unity.Services.CloudSave.Exceptions;
using UnityEngine;

[assembly: InternalsVisibleTo("Unity.Services.CloudSave.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace Unity.Services.CloudSave
{
    interface ISaveDataClient
    {
        Task<List<string>> RetrieveAllKeysAsync();
        Task ForceSaveAsync(Dictionary<string, object> data);
        Task ForceDeleteAsync(string key);
        Task<Dictionary<string, string>> LoadAsync(HashSet<string> keys = null);
    }

    internal class SaveDataClient : ISaveDataClient
    {
        readonly IApiClient m_ApiClient;
        readonly ICloudSaveApiErrorHandler m_ErrorHandler;

        internal SaveDataClient(IApiClient apiClient, ICloudSaveApiErrorHandler errorHandler)
        {
            m_ApiClient = apiClient;
            m_ErrorHandler = errorHandler;
        }

        public async Task<List<string>> RetrieveAllKeysAsync()
        {
            try
            {
                List<string> returnSet = new List<string>();
                Response<GetKeysResponse> response;
                string lastAddedKey = null;
                do
                {
                    response = await m_ApiClient.RetrieveKeysAsync(lastAddedKey);
                    List<KeyMetadata> items = response.Result.Results;
                    if (items.Count > 0)
                    {
                        foreach (var item in items)
                        {
                            returnSet.Add(item.Key);
                        }

                        lastAddedKey = items[items.Count - 1].Key;
                    }
                } while (!string.IsNullOrEmpty(response.Result.Links.Next));

                return returnSet;
            }
            catch (HttpException<BasicErrorResponse> e)
            {
                throw m_ErrorHandler.HandleBasicResponseException(e);
            }
            catch (HttpException<ValidationErrorResponse> e)
            {
                throw m_ErrorHandler.HandleValidationResponseException(e);
            }
            catch (ResponseDeserializationException e)
            {
                throw m_ErrorHandler.HandleDeserializationException(e);
            }
            catch (HttpException e)
            {
                throw m_ErrorHandler.HandleHttpException(e);
            }
            catch (Exception e)
            {
                if (e is CloudSaveException)
                {
                    throw;
                }
                
                throw m_ErrorHandler.HandleException(e);
            }
        }

        public async Task ForceSaveAsync(Dictionary<string, object> data)
        {
            try
            {
                if (data.Count > 0)
                {
                    await m_ApiClient.ForceSaveAsync(data);
                }
            }
            catch (HttpException<BasicErrorResponse> e)
            {
                throw m_ErrorHandler.HandleBasicResponseException(e);
            }
            catch (HttpException<BatchValidationErrorResponse> e)
            {
                throw m_ErrorHandler.HandleBatchValidationResponseException(e);
            }
            catch (ResponseDeserializationException e)
            {
                throw m_ErrorHandler.HandleDeserializationException(e);
            }
            catch (HttpException e)
            {
                throw m_ErrorHandler.HandleHttpException(e);
            }
            catch (Exception e)
            {
                if (e is CloudSaveException)
                {
                    throw;
                }
                
                throw m_ErrorHandler.HandleException(e);
            }
        }

        public async Task ForceDeleteAsync(string key)
        {
            try
            {
                await m_ApiClient.ForceDeleteAsync(key);
            }
            catch (HttpException<BasicErrorResponse> e)
            {
                throw m_ErrorHandler.HandleBasicResponseException(e);
            }
            catch (HttpException<ValidationErrorResponse> e)
            {
                throw m_ErrorHandler.HandleValidationResponseException(e);
            }
            catch (ResponseDeserializationException e)
            {
                throw m_ErrorHandler.HandleDeserializationException(e);
            }
            catch (HttpException e)
            {
                throw m_ErrorHandler.HandleHttpException(e);
            }
            catch (Exception e)
            {
                if (e is CloudSaveException)
                {
                    throw;
                }
                
                throw m_ErrorHandler.HandleException(e);
            }
        }

        public async Task<Dictionary<string, string>> LoadAsync(HashSet<string> keys = null)
        {
            var result = new Dictionary<string, string>();
            try
            {
                Response<GetItemsResponse> response;
                string lastAddedKey = null;
                do
                {
                    response = await m_ApiClient.LoadAsync(keys, lastAddedKey);
                    List<Item> items = response.Result.Results;
                    if (items.Count > 0)
                    {
                        foreach (var item in items)
                        {
                            result[item.Key] = item.Value.GetAsString();
                        }

                        lastAddedKey = items[items.Count - 1].Key;
                    }
                } while (!string.IsNullOrEmpty(response.Result.Links.Next));

                return result;
            }
            catch (HttpException<BasicErrorResponse> e)
            {
                throw m_ErrorHandler.HandleBasicResponseException(e);
            }
            catch (HttpException<ValidationErrorResponse> e)
            {
                throw m_ErrorHandler.HandleValidationResponseException(e);
            }
            catch (ResponseDeserializationException e)
            {
                throw m_ErrorHandler.HandleDeserializationException(e);
            }
            catch (HttpException e)
            {
                throw m_ErrorHandler.HandleHttpException(e);
            }
            catch (Exception e)
            {
                if (e is CloudSaveException)
                {
                    throw;
                }
                
                throw m_ErrorHandler.HandleException(e);
            }
        }
    }
}