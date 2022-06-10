using System;
using Unity.Services.Core;

namespace Unity.Services.CloudSave
{
    /// <summary>
    /// The entry class to the Cloud Save service.
    /// </summary>
    public static class CloudSaveService
    {
        internal static ICloudSaveService instance;

        /// <summary>
        /// The default singleton instance to access the Cloud Save service.
        /// </summary>
        /// <exception cref="ServicesInitializationException">
        /// This exception is thrown if the <code>UnityServices.InitializeAsync()</code>
        /// has not finished before accessing the singleton.
        /// </exception>
        public static ICloudSaveService Instance
        {
            get
            {
                if (instance == null)
                {
                    throw new ServicesInitializationException("The Cloud Save service has not been initialized. Please initialize Unity Services.");
                }

                return instance;
            }
        }
    }

    public class CloudSaveServiceInstance : ICloudSaveService
    {
        public ICloudSaveDataClient Data { get; internal set; }

        internal CloudSaveServiceInstance(ICloudSaveDataClient data)
        {
            Data = data;
        }
    }

    public interface ICloudSaveService
    {
        ICloudSaveDataClient Data { get; }
    }
}
