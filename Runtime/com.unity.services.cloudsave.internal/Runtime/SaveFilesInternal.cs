using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Unity.Services.CloudSave.Internal.Http;
using Unity.Services.CloudSave.Internal.Models;
using FileItem = Unity.Services.CloudSave.Models.FileItem;

namespace Unity.Services.CloudSave.Internal
{
    class SaveFilesInternal : ICloudSaveFilesClient
    {
        readonly IFilesApiClient m_FilesApiClient;
        readonly ICloudSaveApiErrorHandlerV2 _mErrorHandlerV2;

        internal SaveFilesInternal(IFilesApiClient filesApiClient, ICloudSaveApiErrorHandlerV2 errorHandlerV2)
        {
            m_FilesApiClient = filesApiClient;
            _mErrorHandlerV2 = errorHandlerV2;
        }

        public async Task<List<FileItem>> ListAllAsync()
        {
            return await RunWithErrorHandling(async() =>
            {
                List<FileItem> returnSet = new List<FileItem>();
                Response<FileList> response;
                string lastAddedKey = null;
                do
                {
                    response = await m_FilesApiClient.ListAsync(lastAddedKey);
                    List<FileItem> items = response.Result.Results.ConvertAll(i => new FileItem(i));
                    if (items.Count > 0)
                    {
                        returnSet.AddRange(items);
                        lastAddedKey = response.Result.Links.Next;
                    }
                }
                while (!string.IsNullOrEmpty(response.Result.Links.Next));

                return returnSet;
            });
        }

        public async Task<FileItem> GetMetadataAsync(string key)
        {
            return await RunWithErrorHandling(async() =>
            {
                var metadataResponse = await m_FilesApiClient.GetMetadataAsync(key);
                return new FileItem(metadataResponse.Result);
            });
        }

        public async Task SaveAsync(string key, Stream stream, SaveOptions options = null)
        {
            await RunWithErrorHandling(async() =>
            {
                var uploadUrlResponse = await m_FilesApiClient.GetUploadUrlAsync(key, stream, options?.WriteLock);
                return await m_FilesApiClient.UploadAsync(stream, uploadUrlResponse.Result);
            });
        }

        public async Task SaveAsync(string key, byte[] bytes, SaveOptions options = null)
        {
            await RunWithErrorHandling(async() =>
            {
                var uploadUrlResponse = await m_FilesApiClient.GetUploadUrlAsync(key, bytes, options?.WriteLock);
                return await m_FilesApiClient.UploadAsync(bytes, uploadUrlResponse.Result);
            });
        }

        public async Task<Stream> LoadStreamAsync(string key)
        {
            return await RunWithErrorHandling(async() =>
            {
                var downloadUrlResponse = await m_FilesApiClient.GetDownloadUrlAsync(key);
                var fileStreamResponse = await m_FilesApiClient.DownloadStreamAsync(downloadUrlResponse.Result);

                return fileStreamResponse.Result;
            });
        }

        public async Task<byte[]> LoadBytesAsync(string key)
        {
            return await RunWithErrorHandling(async() =>
            {
                var downloadUrlResponse = await m_FilesApiClient.GetDownloadUrlAsync(key);
                var fileStreamResponse = await m_FilesApiClient.DownloadBytesAsync(downloadUrlResponse.Result);

                return fileStreamResponse.Result;
            });
        }

        public async Task DeleteAsync(string key, DeleteOptions options = null)
        {
            await RunWithErrorHandling(async() =>
            {
                await m_FilesApiClient.DeleteAsync(key, options?.WriteLock);
                return true;
            });
        }

        async Task<T> RunWithErrorHandling<T>(Func<Task<T>> func)
        {
            try
            {
                if (_mErrorHandlerV2.IsRateLimited)
                {
                    throw _mErrorHandlerV2.CreateRateLimitException();
                }

                return await func();
            }
            catch (HttpException<BasicErrorResponse> e)
            {
                throw _mErrorHandlerV2.HandleBasicResponseException(e);
            }
            catch (HttpException<ValidationErrorResponse> e)
            {
                throw _mErrorHandlerV2.HandleValidationResponseException(e);
            }
            catch (HttpException<GCSErrorResponse> e)
            {
                throw _mErrorHandlerV2.HandleGCSResponseException(e);
            }
            catch (ResponseDeserializationException e)
            {
                throw _mErrorHandlerV2.HandleDeserializationException(e);
            }
            catch (HttpException e)
            {
                throw _mErrorHandlerV2.HandleHttpException(e);
            }
            catch (Exception e)
            {
                if (e is CloudSaveException)
                {
                    throw;
                }

                throw _mErrorHandlerV2.HandleException(e);
            }
        }
    }
}
