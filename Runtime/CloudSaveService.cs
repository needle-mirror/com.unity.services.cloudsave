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
        /// This exception is thrown if the <c>UnityServices.InitializeAsync()</c>
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

    /// <summary>
    /// The current instance of the Cloud Save Service.
    /// </summary>
    public class CloudSaveServiceInstance : ICloudSaveService
    {
        /// <summary>
        /// The Data API service.
        /// </summary>
        public IDataService Data { get; }

        /// <summary>
        /// The Files API service.
        /// </summary>
        public IFilesService Files { get; }

        internal CloudSaveServiceInstance(IDataService data, IFilesService files)
        {
            Data = data;
            Files = files;
        }
    }


    /// <summary>
    /// Interface for Cloud Save Service.
    /// </summary>
    public interface ICloudSaveService
    {
        /// <summary>
        /// Interface for the Data API service.
        /// </summary>
        IDataService Data { get; }

        /// <summary>
        /// Interface for the Files API service.
        /// </summary>
        IFilesService Files { get; }
    }
}
