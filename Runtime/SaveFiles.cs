using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using FileItem = Unity.Services.CloudSave.Models.FileItem;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace Unity.Services.CloudSave
{
    public class WriteLockOptions
    {
        public string WriteLock { get; set; }
    }

    public class SaveOptions : WriteLockOptions {}
    public class DeleteOptions : WriteLockOptions {}

    public interface ICloudSaveFilesClient
    {
        /// <summary>
        /// Returns all player-scoped files stored in Cloud Save for the logged in player.
        /// Throws a CloudSaveException with a reason code and explanation of what happened.
        ///
        /// This method includes pagination.
        /// </summary>
        /// <returns>A list of file metadata for the files stored in Cloud Save for the logged in player.</returns>
        /// <exception cref="CloudSaveException">Thrown if request is unsuccessful.</exception>
        /// <exception cref="CloudSaveValidationException">Thrown if the service returned validation error.</exception>
        /// <exception cref="CloudSaveRateLimitedException">Thrown if the service returned rate limited error.</exception>
        Task<List<FileItem>> ListAllAsync();

        /// <summary>
        /// Returns the metadata of a file stored in Cloud Save for the logged in player.
        /// Throws a CloudSaveException with a reason code and explanation of what happened.
        /// </summary>
        /// <returns>The metadata of the specified file stored in Cloud Save for the logged in player</returns>
        /// <exception cref="CloudSaveException">Thrown if request is unsuccessful.</exception>
        /// <exception cref="CloudSaveValidationException">Thrown if the service returned validation error.</exception>
        /// <exception cref="CloudSaveRateLimitedException">Thrown if the service returned rate limited error.</exception>
        Task<FileItem> GetMetadataAsync(string key);

        /// <summary>
        /// Upload a player-scoped file to the Cloud Save service, overwriting if the file already exists.
        /// File name can only contain alphanumeric characters, dashes, and underscores and be up to a length of 255 characters.
        /// Throws a CloudSaveException with a reason code and explanation of what happened.
        ///
        /// </summary>
        /// <param name="key">The key at which to upload the file</param>
        /// <param name="stream">The Stream containing the file data</param>
        /// <param name="options">Options object with "WriteLock", the expected stored writeLock of the file - if this value is provided and is not a match then the operation will not succeed. If it is not provided then the operation will be performed regardless of the stored writeLock value.</param>
        /// <exception cref="CloudSaveException">Thrown if request is unsuccessful.</exception>
        /// <exception cref="CloudSaveValidationException">Thrown if the service returned validation error.</exception>
        /// <exception cref="CloudSaveRateLimitedException">Thrown if the service returned rate limited error.</exception>
        Task SaveAsync(string key, Stream stream, SaveOptions options = null);

        /// <summary>
        /// Upload a player-scoped file to the Cloud Save service, overwriting if the file already exists.
        /// File name can only contain alphanumeric characters, dashes, and underscores and be up to a length of 255 characters.
        /// Throws a CloudSaveException with a reason code and explanation of what happened.
        ///
        /// </summary>
        /// <param name="key">The key at which to upload the file</param>
        /// <param name="bytes">The byte array containing the file data</param>
        /// <param name="options">Options object with "WriteLock", the expected stored writeLock of the file - if this value is provided and is not a match then the operation will not succeed. If it is not provided then the operation will be performed regardless of the stored writeLock value.</param>
        /// <exception cref="CloudSaveException">Thrown if request is unsuccessful.</exception>
        /// <exception cref="CloudSaveValidationException">Thrown if the service returned validation error.</exception>
        /// <exception cref="CloudSaveRateLimitedException">Thrown if the service returned rate limited error.</exception>
        Task SaveAsync(string key, byte[] bytes, SaveOptions options = null);

        /// <summary>
        /// Upload a player-scoped file to the Cloud Save service, overwriting if the file already exists.
        /// File name can only contain alphanumeric characters, dashes, and underscores and be up to a length of 255 characters.
        /// Throws a CloudSaveException with a reason code and explanation of what happened.
        ///
        /// </summary>
        /// <returns>A Stream containing the downloaded file data</returns>
        /// <param name="key">The key of the saved file to be loaded.</param>
        /// <exception cref="CloudSaveException">Thrown if request is unsuccessful.</exception>
        /// <exception cref="CloudSaveValidationException">Thrown if the service returned validation error.</exception>
        /// <exception cref="CloudSaveRateLimitedException">Thrown if the service returned rate limited error.</exception>
        Task<Stream> LoadStreamAsync(string key);

        /// <summary>
        /// Upload a player-scoped file to the Cloud Save service, overwriting if the file already exists.
        /// File name can only contain alphanumeric characters, dashes, and underscores and be up to a length of 255 characters.
        /// Throws a CloudSaveException with a reason code and explanation of what happened.
        ///
        /// </summary>
        /// <returns>A byte array containing the downloaded file data</returns>
        /// <param name="key">The key of the saved file to be loaded.</param>
        /// <exception cref="CloudSaveException">Thrown if request is unsuccessful.</exception>
        /// <exception cref="CloudSaveValidationException">Thrown if the service returned validation error.</exception>
        /// <exception cref="CloudSaveRateLimitedException">Thrown if the service returned rate limited error.</exception>
        Task<byte[]> LoadBytesAsync(string key);

        /// <summary>
        /// Delete a player-scoped file form the Cloud Save service.
        /// File name can only contain alphanumeric characters, dashes, and underscores and be up to a length of 255 characters.
        /// Throws a CloudSaveException with a reason code and explanation of what happened.
        ///
        /// </summary>
        /// <param name="key">The key of the saved file to be deleted.</param>
        /// <param name="options">Options object with "WriteLock", the expected stored writeLock of the file - if this value is provided and is not a match then the operation will not succeed. If it is not provided then the operation will be performed regardless of the stored writeLock value.</param>
        /// <exception cref="CloudSaveException">Thrown if request is unsuccessful.</exception>
        /// <exception cref="CloudSaveValidationException">Thrown if the service returned validation error.</exception>
        /// <exception cref="CloudSaveRateLimitedException">Thrown if the service returned rate limited error.</exception>
        Task DeleteAsync(string key, DeleteOptions options = null);
    }
}
