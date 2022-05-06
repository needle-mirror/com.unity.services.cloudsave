using System;
using Unity.Services.Core;

namespace Unity.Services.CloudSave
{
    public static class CloudSaveService
    {
        internal static ICloudSaveService instance;

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
