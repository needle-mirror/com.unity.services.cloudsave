using System.Threading.Tasks;
using Unity.Services.Authentication.Internal;
using Unity.Services.CloudSave.Internal;
using Unity.Services.CloudSave.Internal.Apis.Data;
using Unity.Services.CloudSave.Internal.Apis.Files;
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
            var config = new Configuration(GetHost(projectConfiguration), null, null, null);

            var dataApiClient = new DataApiClient(new HttpClient(), accessToken, config);
            var filesApiClient = new FilesApiClient(new HttpClient(), accessToken, config);

            var authentication = new AuthenticationWrapper(playerId, accessToken);

            var internalPlayerDataApiClient =
                new PlayerDataApiClient(cloudProjectId, authentication, dataApiClient);
            var internalCustomDataApiClient =
                new CustomDataApiClient(cloudProjectId, authentication, dataApiClient);
            var internalFilesApiClient =
                new PlayerFilesApiClient(cloudProjectId, authentication, filesApiClient);

            var dataService = new DataService(
                internalPlayerDataApiClient,
                new CloudSaveApiErrorHandlerDepr(new RateLimiter()),
                new PlayerDataService(internalPlayerDataApiClient, new ApiErrorHandler(new RateLimiter())),
                new CustomDataService(internalCustomDataApiClient, new ApiErrorHandler(new RateLimiter()))
            );

            var filesService = new FilesService(
                new PlayerFilesService(internalFilesApiClient, new ApiErrorHandler(new RateLimiter()))
            );

            CloudSaveService.instance = new CloudSaveServiceInstance(dataService, filesService);

            return Task.CompletedTask;
        }

        static string GetHost(IProjectConfiguration projectConfiguration)
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
