using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Unity.Services.CloudSave.Internal.Data;
using Unity.Services.CloudSave.Internal.Models;
using Unity.Services.CloudSave.Models;
using Unity.Services.Core;
using Unity.Services.Core.Configuration.Internal;
using FieldFilter = Unity.Services.CloudSave.Internal.Models.FieldFilter;

[assembly: InternalsVisibleTo("Unity.Services.CloudSave.Tests")]

namespace Unity.Services.CloudSave.Internal
{
    /// <summary>
    /// Wraps the generated API client to automatically pass the project ID into the requests.
    /// </summary>
    interface ICustomDataApiClient
    {
        Task<Response<GetKeysResponse>> ListKeysAsync(string customID, string afterKey);
        Task<Response<GetItemsResponse>> LoadAsync(string customID, ISet<string> keys, string afterKey);
        Task<Response<QueryIndexResponse>> QueryAsync(Query query, AccessClass accessClass = AccessClass.Default);
    }

    class CustomDataApiClient : ICustomDataApiClient
    {
        readonly ICloudProjectId m_CloudProjectId;
        readonly Internal.Apis.Data.IDataApiClient m_DataClient;
        readonly IAuthentication m_Authentication;

        internal CustomDataApiClient(ICloudProjectId cloudProjectId, IAuthentication authentication,
                                     Internal.Apis.Data.IDataApiClient dataClient)
        {
            m_CloudProjectId = cloudProjectId;
            m_DataClient = dataClient;
            m_Authentication = authentication;
        }

        public async Task<Response<GetKeysResponse>> ListKeysAsync(string customID, string afterKey)
        {
            ValidateRequiredDependencies();
            var request = new GetCustomKeysRequest(m_CloudProjectId.GetCloudProjectId(), customID, afterKey);

            return await m_DataClient.GetCustomKeysAsync(request);
        }

        public async Task<Response<GetItemsResponse>> LoadAsync(string customID, ISet<string> keys, string afterKey)
        {
            ValidateRequiredDependencies();
            var requestedKeys = (null == keys) ? null : new List<string>(keys);

            var request =
                new GetCustomItemsRequest(m_CloudProjectId.GetCloudProjectId(), customID, requestedKeys, afterKey);

            return await m_DataClient.GetCustomItemsAsync(request);
        }

        public async Task<Response<QueryIndexResponse>> QueryAsync(Query query,
            AccessClass accessClass = AccessClass.Default)
        {
            ValidateRequiredDependencies();
            var queryFields = query?.Fields?.Select(FieldFilterToInternalFieldFilter).ToList() ??
                new List<FieldFilter>();
            var queryKeys = query?.ReturnKeys?.ToList() ?? new List<string>();
            var offset = query?.Offset ?? 0;
            var limit = query?.Limit ?? 0;

            switch (accessClass)
            {
                case AccessClass.Default:
                    var request = new QueryDefaultCustomDataRequest(m_CloudProjectId.GetCloudProjectId(),
                        new QueryIndexBody(queryFields, queryKeys, offset, limit));
                    return await m_DataClient.QueryDefaultCustomDataAsync(request);
                case AccessClass.Public:
                case AccessClass.Private:
                case AccessClass.Protected:
                default:
                    throw new InvalidOperationException(
                        "QueryAsync can only be called with Default AccessClass");
            }
        }

        void ValidateRequiredDependencies()
        {
            if (string.IsNullOrEmpty(m_CloudProjectId.GetCloudProjectId()))
            {
                throw new CloudSaveException(CloudSaveExceptionReason.ProjectIdMissing, CommonErrorCodes.Unknown,
                    "Project ID is missing - make sure the project is correctly linked to your game and try again.",
                    null);
            }

            if (string.IsNullOrEmpty(m_Authentication.GetAccessToken()))
            {
                throw new CloudSaveException(CloudSaveExceptionReason.AccessTokenMissing, CommonErrorCodes.InvalidToken,
                    "Access token is missing - ensure you are signed in through the Authentication SDK and try again.",
                    null);
            }
        }

        static FieldFilter FieldFilterToInternalFieldFilter(CloudSave.Models.FieldFilter fieldFilter)
        {
            return new FieldFilter(fieldFilter.Key, fieldFilter.Value, (FieldFilter.OpOptions)fieldFilter.Op,
                fieldFilter.Asc);
        }
    }
}
