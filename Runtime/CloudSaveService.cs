using Unity.Services.CloudSave.Internal;
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
        public IDataService Data { get; }
        public IFilesService Files { get; }

        internal CloudSaveServiceInstance(IDataService data, IFilesService files)
        {
            Data = data;
            Files = files;
        }
    }

    public interface ICloudSaveService
    {
        IDataService Data { get; }
        IFilesService Files { get; }
    }
}
