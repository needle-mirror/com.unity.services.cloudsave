using Unity.Services.CloudSave;

namespace Unity.Services.Core
{
    /// <summary>
    /// Cloud Save Services Extension Methods
    /// </summary>
    public static class UnityServicesExtensions
    {
        /// <summary>
        /// Retrieve the cloud save service from the core service registry
        /// </summary>
        /// <param name="unityServices">The core services instance</param>
        /// <returns>The cloud save service instance</returns>
        public static ICloudSaveService GetCloudSaveService(this IUnityServices unityServices)
        {
            return unityServices.GetService<ICloudSaveService>();
        }
    }
}
