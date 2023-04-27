using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Unity.Services.CloudSave.Internal.Files;
using Unity.Services.CloudSave.Internal.Models;
using Unity.Services.Core;
using Unity.Services.Core.Configuration.Internal;

[assembly: InternalsVisibleTo("Unity.Services.CloudSave.Tests")]

namespace Unity.Services.CloudSave.Internal
{
    interface IFilesApiClient
    {
        Task<Response<FileList>> ListAsync(string afterKey);

        Task<Response<FileItem>> GetMetadataAsync(string afterKey);

        Task<Response<SignedUrlResponse>> GetUploadUrlAsync(string key, Stream stream, string writeLock = null);

        Task<Response<SignedUrlResponse>> GetUploadUrlAsync(string key, byte[] bytes, string writeLock = null);

        Task<Response<SignedUrlResponse>> GetDownloadUrlAsync(string key);

        Task<Response> UploadAsync(Stream stream, SignedUrlResponse uploadParams);

        Task<Response> UploadAsync(byte[] bytes, SignedUrlResponse uploadParams);

        Task<Response<MemoryStream>> DownloadStreamAsync(SignedUrlResponse uploadParams);

        Task<Response<byte[]>> DownloadBytesAsync(SignedUrlResponse uploadParams);

        Task DeleteAsync(string key, string writeLock = null);
    }

    class FilesApiClient : IFilesApiClient
    {
        readonly ICloudProjectId m_CloudProjectId;
        readonly Internal.Apis.Files.IFilesApiClient m_FilesClient;
        readonly IAuthentication m_Authentication;

        const int k_keyMaxLength = 255;
        const string k_keyRegex = "^[A-Za-z0-9-_][A-Za-z0-9-_.]{0,254}$";

        internal FilesApiClient(ICloudProjectId cloudProjectId, IAuthentication authentication, Internal.Apis.Files.IFilesApiClient filesClient)
        {
            m_CloudProjectId = cloudProjectId;
            m_FilesClient = filesClient;
            m_Authentication = authentication;
        }

        public async Task<Response<FileList>> ListAsync(string afterKey)
        {
            ValidateRequiredDependencies();

            if (!string.IsNullOrEmpty(afterKey))
            {
                ValidateKey(afterKey);
            }

            ListFilesRequest request = new ListFilesRequest(m_CloudProjectId.GetCloudProjectId(),
                m_Authentication.GetPlayerId(), afterKey);

            return await m_FilesClient.ListFilesAsync(request);
        }

        public async Task<Response<FileItem>> GetMetadataAsync(string key)
        {
            ValidateRequiredDependencies();

            ValidateKey(key);

            GetFileMetadataRequest request = new GetFileMetadataRequest(m_CloudProjectId.GetCloudProjectId(),
                m_Authentication.GetPlayerId(), key);

            return await m_FilesClient.GetFileMetadataAsync(request);
        }

        public async Task<Response<SignedUrlResponse>> GetUploadUrlAsync(string key, Stream stream, string writeLock = null)
        {
            ValidateRequiredDependencies();

            ValidateKey(key);

            ValidateStream(stream);

            stream.Position = 0;

            string md5Hash;
            using (var md5 = MD5.Create())
            {
                var md5Bytes = md5.ComputeHash(stream);
                md5Hash = Convert.ToBase64String(md5Bytes);
                stream.Position = 0;
            }

            GetUploadUrlRequest request = new GetUploadUrlRequest(key,
                m_CloudProjectId.GetCloudProjectId(),
                m_Authentication.GetPlayerId(), new FileDetails(MediaTypeNames.Application.Octet, stream.Length, md5Hash, writeLock));

            return await m_FilesClient.GetUploadUrlAsync(request);
        }

        public async Task<Response<SignedUrlResponse>> GetUploadUrlAsync(string key, byte[] bytes, string writeLock = null)
        {
            ValidateRequiredDependencies();

            ValidateKey(key);

            string md5Hash;
            using (var md5 = MD5.Create())
            {
                var md5Bytes = md5.ComputeHash(bytes);
                md5Hash = Convert.ToBase64String(md5Bytes);
            }

            GetUploadUrlRequest request = new GetUploadUrlRequest(key,
                m_CloudProjectId.GetCloudProjectId(),
                m_Authentication.GetPlayerId(), new FileDetails(MediaTypeNames.Application.Octet, bytes.Length, md5Hash, writeLock));

            return await m_FilesClient.GetUploadUrlAsync(request);
        }

        public async Task<Response<SignedUrlResponse>> GetDownloadUrlAsync(string key)
        {
            ValidateRequiredDependencies();

            ValidateKey(key);

            GetDownloadUrlRequest request = new GetDownloadUrlRequest(key,
                m_CloudProjectId.GetCloudProjectId(),
                m_Authentication.GetPlayerId());

            return await m_FilesClient.GetDownloadUrlAsync(request);
        }

        public async Task<Response> UploadAsync(Stream stream, SignedUrlResponse uploadParams)
        {
            ValidateRequiredDependencies();

            return await m_FilesClient.UploadFileAsync(stream, uploadParams);
        }

        public async Task<Response> UploadAsync(byte[] bytes, SignedUrlResponse uploadParams)
        {
            ValidateRequiredDependencies();

            return await m_FilesClient.UploadFileAsync(bytes, uploadParams);
        }

        public async Task<Response<MemoryStream>> DownloadStreamAsync(SignedUrlResponse downloadParams)
        {
            ValidateRequiredDependencies();

            return await m_FilesClient.DownloadFileStreamAsync(downloadParams);
        }

        public async Task<Response<byte[]>> DownloadBytesAsync(SignedUrlResponse downloadParams)
        {
            ValidateRequiredDependencies();

            return await m_FilesClient.DownloadFileBytesAsync(downloadParams);
        }

        public async Task DeleteAsync(string key, string writeLock = null)
        {
            ValidateKey(key);

            ValidateRequiredDependencies();
            DeleteFileRequest request = new DeleteFileRequest(key,
                m_CloudProjectId.GetCloudProjectId(),
                m_Authentication.GetPlayerId(),
                writeLock);

            await m_FilesClient.DeleteFileAsync(request);
        }

        static void ValidateKey(string key)
        {
            if (key == null || key.Length > k_keyMaxLength || !Regex.IsMatch(key, k_keyRegex))
            {
                throw new CloudSaveValidationException(CloudSaveExceptionReason.InvalidArgument, 1004,
                    "There was a validation error. Check 'Details' for more information.",
                    new List<CloudSaveValidationErrorDetail>
                    {
                        new CloudSaveValidationErrorDetail("key",
                            new List<string>
                            {
                                "invalid key. it must only contain alphanumeric characters, underscores or dashes, and it should not start with a period. its length must be between 1 and 255 characters"
                            })
                    }, null);
            }
        }

        static void ValidateStream(Stream stream)
        {
            if (stream == null || !stream.CanSeek || !stream.CanRead || stream.Length == 0)
                throw new CloudSaveValidationException(CloudSaveExceptionReason.InvalidArgument, 1004,
                    "There was a validation error. Check 'Details' for more information.",
                    new List<CloudSaveValidationErrorDetail>
                    {
                        new CloudSaveValidationErrorDetail("stream",
                            new List<string>
                            {
                                "invalid stream. make sure the stream is non-null, non-empty, readable, and seekable."
                            })
                    }, null);
        }

        void ValidateRequiredDependencies()
        {
            if (String.IsNullOrEmpty(m_CloudProjectId.GetCloudProjectId()))
            {
                throw new CloudSaveException(CloudSaveExceptionReason.ProjectIdMissing, CommonErrorCodes.Unknown,
                    "Project ID is missing - make sure the project is correctly linked to your game and try again.", null);
            }

            if (String.IsNullOrEmpty(m_Authentication.GetPlayerId()))
            {
                throw new CloudSaveException(CloudSaveExceptionReason.PlayerIdMissing, CommonErrorCodes.Unknown,
                    "Player ID is missing - ensure you are signed in through the Authentication SDK and try again.", null);
            }

            if (String.IsNullOrEmpty(m_Authentication.GetAccessToken()))
            {
                throw new CloudSaveException(CloudSaveExceptionReason.AccessTokenMissing, CommonErrorCodes.InvalidToken,
                    "Access token is missing - ensure you are signed in through the Authentication SDK and try again.", null);
            }
        }
    }
}
