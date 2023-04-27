using System.Threading.Tasks;
using Unity.Services.Authentication.Internal;
using Unity.Services.CloudSave.Internal;
using Unity.Services.CloudSave.Internal.Http;
using Unity.Services.Core.Internal;
using Unity.Services.Core.Device.Internal;
using UnityEngine;
using Unity.Services.Core.Configuration.Internal;

namespace Unity.Services.CloudSave
{
    internal class CloudSaveInitializer : IInitializablePackage
    {
        const string k_CloudEnvironmentKey = "com.unity.services.core.cloud-environment";
        const string k_StagingEnvironment = "staging";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Register()
        {
            CoreRegistry.Instance.RegisterPackage(new CloudSaveInitializer())
                .DependsOn<ICloudProjectId>()
                .DependsOn<IPlayerId>()
                .DependsOn<IInstallationId>()
                .DependsOn<IProjectConfiguration>();
        }

        public Task Initialize(CoreRegistry registry)
        {
            var projectConfiguration = registry.GetServiceComponent<IProjectConfiguration>();
            var cloudProjectId = registry.GetServiceComponent<ICloudProjectId>();
            var accessToken = registry.GetServiceComponent<IAccessToken>();
            var playerId = registry.GetServiceComponent<IPlayerId>();

            Internal.Apis.Data.IDataApiClient internalDataApiClient = new Internal.Apis.Data.DataApiClient(new HttpClient(), accessToken,
                new Configuration(GetHost(projectConfiguration), null, null, null));
            Internal.Apis.Files.IFilesApiClient internalFilesApiClient = new Internal.Apis.Files.FilesApiClient(new HttpClient(), accessToken,
                new Configuration(GetHost(projectConfiguration), null, null, null));

            IAuthentication authentication = new AuthenticationWrapper(playerId, accessToken);

            IDataApiClient dataApiClient = new DataApiClient(cloudProjectId, authentication, internalDataApiClient);
            IFilesApiClient filesApiClient =
                new FilesApiClient(cloudProjectId, authentication, internalFilesApiClient);

            CloudSaveService.instance = new CloudSaveServiceInstance(
                new SaveDataInternal(dataApiClient, new CloudSaveApiErrorHandler(new RateLimiter())),
                new SaveFilesInternal(filesApiClient, new CloudSaveApiErrorHandlerV2(new RateLimiter())));

            return Task.CompletedTask;
        }

        string GetHost(IProjectConfiguration projectConfiguration)
        {
            var cloudEnvironment = projectConfiguration?.GetString(k_CloudEnvironmentKey);

            switch (cloudEnvironment)
            {
                case k_StagingEnvironment:
                    return "https://cloud-save-stg.services.api.unity.com";
                default:
                    return "https://cloud-save.services.api.unity.com";
            }
        }
    }
}
