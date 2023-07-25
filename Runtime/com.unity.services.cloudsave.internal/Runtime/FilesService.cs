using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FileItem = Unity.Services.CloudSave.Models.FileItem;

namespace Unity.Services.CloudSave.Internal
{
    public interface IFilesService
    {
        IPlayerFilesService Player { get; }

        #region Deprecated, pre v3.0.0
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
        [Obsolete("This method will be removed in an upcoming release. Use Task<List<FileItem>> CloudSaveService.Instance.Files.Player.ListAllAsync() instead.", false)]
        Task<List<FileItem>> ListAllAsync();

        /// <summary>
        /// Returns the metadata of a file stored in Cloud Save for the logged in player.
        /// Throws a CloudSaveException with a reason code and explanation of what happened.
        /// </summary>
        /// <returns>The metadata of the specified file stored in Cloud Save for the logged in player</returns>
        /// <exception cref="CloudSaveException">Thrown if request is unsuccessful.</exception>
        /// <exception cref="CloudSaveValidationException">Thrown if the service returned validation error.</exception>
        /// <exception cref="CloudSaveRateLimitedException">Thrown if the service returned rate limited error.</exception>
        [Obsolete("This method will be removed in an upcoming release. Use Task<FileItem> CloudSaveService.Instance.Files.Player.GetMetadataAsync(string key) instead.", false)]
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
        [Obsolete("This method will be removed in an upcoming release. Use Task CloudSaveService.Instance.Files.Player.SaveAsync(string key, Stream stream, SaveOptions options = null) instead.", false)]
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
        [Obsolete("This method will be removed in an upcoming release. Use Task CloudSaveService.Instance.Files.Player.SaveAsync(string key, byte[] bytes, SaveOptions options = null) instead.", false)]
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
        [Obsolete("This method will be removed in an upcoming release. Use Task<Stream> CloudSaveService.Instance.Files.Player.LoadStreamAsync(string key) instead.", false)]
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
        [Obsolete("This method will be removed in an upcoming release. Use Task<byte[]> CloudSaveService.Instance.Files.Player.LoadBytesAsync(string key) instead.", false)]
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
        [Obsolete("This method will be removed in an upcoming release. Use Task CloudSaveService.Instance.Files.Player.DeleteAsync(string key, DeleteOptions options = null) instead.", false)]
        Task DeleteAsync(string key, DeleteOptions options = null);
        #endregion
    }

    class FilesService : IFilesService
    {
        internal FilesService(IPlayerFilesService playerFilesService)
        {
            Player = playerFilesService;
        }

        public IPlayerFilesService Player { get; }

        #region Deprecated from namespace, pre v3.0.0, but implementation is unchanged
        public async Task<List<FileItem>> ListAllAsync()
        {
            return await Player.ListAllAsync();
        }

        public async Task<FileItem> GetMetadataAsync(string key)
        {
            return await Player.GetMetadataAsync(key);
        }

        public async Task SaveAsync(string key, Stream stream, SaveOptions options = null)
        {
            await Player.SaveAsync(key, stream, options);
        }

        public async Task SaveAsync(string key, byte[] bytes, SaveOptions options = null)
        {
            await Player.SaveAsync(key, bytes, options);
        }

        public async Task<Stream> LoadStreamAsync(string key)
        {
            return await Player.LoadStreamAsync(key);
        }

        public async Task<byte[]> LoadBytesAsync(string key)
        {
            return await Player.LoadBytesAsync(key);
        }

        public async Task DeleteAsync(string key, DeleteOptions options = null)
        {
            await Player.DeleteAsync(key, options);
        }

        #endregion
    }
}
