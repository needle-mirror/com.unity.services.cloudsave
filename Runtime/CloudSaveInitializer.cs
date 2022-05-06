using System.Threading.Tasks;
using Unity.Services.CloudSave.Internal.Apis.Data;
using Unity.Services.CloudSave.Internal.Http;
using Unity.Services.Authentication.Internal;
using Unity.Services.Core.Internal;
using Unity.Services.Core.Device.Internal;
using UnityEngine;

namespace Unity.Services.CloudSave
{
    internal class CloudSaveInitializer : IInitializablePackage
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Register()
        {
            CoreRegistry.Instance.RegisterPackage(new CloudSaveInitializer())
                .DependsOn<IPlayerId>()
                .DependsOn<IInstallationId>();
        }

        public Task Initialize(CoreRegistry registry)
        {
            // This is a temporary fix until the code generator creates services that can be depended on as part of the core init flow
            IDataApiClient cloudSaveDataApiClient = new DataApiClient(new HttpClient(), registry.GetServiceComponent<IAccessToken>());
            // End of temporary fix

            IAuthentication authentication = new AuthenticationWrapper(registry.GetServiceComponent<IPlayerId>(),
                registry.GetServiceComponent<IAccessToken>());

            IApiClient apiClient = new ApiClient(Application.cloudProjectId, authentication, cloudSaveDataApiClient);

            CloudSaveService.instance = new CloudSaveServiceInstance(new SaveDataInternal(apiClient, new CloudSaveApiErrorHandler(new RateLimiter())));

            return Task.CompletedTask;
        }
    }
}
