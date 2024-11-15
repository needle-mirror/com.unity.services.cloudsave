using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Unity.Services.CloudSave
{
    /// <summary>
    /// Obselete interface for saving player data.
    /// </summary>
    [Obsolete("The interface provided by SaveData has moved to CloudSaveService.Instance.Data, and should be accessed from there instead. This API will be removed in an upcoming release.", false)]
    public static class SaveData
    {
        /// <summary>
        /// Obselete method for retrieving multiple player data keys.
        /// </summary>
        /// <returns>A list of keys and their metadata as stored in the server for the logged in player.</returns>
        [Obsolete("The interface provided by SaveData.RetrieveAllKeysAsync() has been replaced by CloudSaveService.Instance.Data.RetrieveAllKeysAsync(), and should be accessed from there instead. This API will be removed in an upcoming release.", false)]
        public static async Task<List<string>> RetrieveAllKeysAsync() => await CloudSaveService.Instance.Data.RetrieveAllKeysAsync();

        /// <summary>
        /// Obselete method for retrieving saving player data.
        /// </summary>
        /// <param name="data">The dictionary of keys and corresponding values to upload</param>
        /// <returns>The dictionary of saved keys and the corresponding updated write lock</returns>
        [Obsolete("The interface provided by SaveData.ForceSaveAsync(Dictionary<string, object>) has been replaced by CloudSaveService.Instance.Data.ForceSaveAsync(Dictionary<string, object>), and should be accessed from there instead. This API will be removed in an upcoming release.", false)]
        public static async Task ForceSaveAsync(Dictionary<string, object> data) => await CloudSaveService.Instance.Data.ForceSaveAsync(data);

        /// <summary>
        /// Obselete method for deleting saving player data.
        /// </summary>
        /// <param name="key">The key to be removed from the server</param>
        /// <returns>Returns void.</returns>
        [Obsolete("The interface provided by SaveData.ForceDeleteAsync(string) has been replaced by CloudSaveService.Instance.Data.ForceDeleteAsync(string), and should be accessed from there instead. This API will be removed in an upcoming release.", false)]
        public static async Task ForceDeleteAsync(string key) => await CloudSaveService.Instance.Data.ForceDeleteAsync(key);

        /// <summary>
        /// Obselete method for retrieving loading player data.
        /// </summary>
        /// <param name="keys">The optional set of keys to load data for</param>
        /// <returns>The dictionary of all key-value pairs that represents the current state of data on the server including their write locks</returns>
        [Obsolete("The interface provided by SaveData.LoadAsync(HashSet<string>) has been replaced by CloudSaveService.Instance.Data.LoadAsync(HashSet<string>), and should be accessed from there instead. This API will be removed in an upcoming release.", false)]
        public static async Task<Dictionary<string, string>> LoadAsync(HashSet<string> keys = null) => await CloudSaveService.Instance.Data.LoadAsync(keys);

        /// <summary>
        /// Obselete method for retrieving loading all player data.
        /// </summary>
        /// <returns>The dictionary of all key-value pairs that represents the current state of data on the server including their write locks</returns>
        [Obsolete("The interface provided by SaveData.LoadAllAsync() has been replaced by CloudSaveService.Instance.Data.LoadAllAsync(), and should be accessed from there instead. This API will be removed in an upcoming release.", false)]
        public static async Task<Dictionary<string, string>> LoadAllAsync() => await CloudSaveService.Instance.Data.LoadAsync();
    }
}
