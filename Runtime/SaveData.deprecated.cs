using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Unity.Services.CloudSave
{
    [Obsolete("The interface provided by SaveData has moved to CloudSaveService.Instance.Data, and should be accessed from there instead. This API will be removed in an upcoming release.", false)]
    public static class SaveData
    {
        [Obsolete("The interface provided by SaveData.RetrieveAllKeysAsync() has been replaced by CloudSaveService.Instance.Data.RetrieveAllKeysAsync(), and should be accessed from there instead. This API will be removed in an upcoming release.", false)]
        public static async Task<List<string>> RetrieveAllKeysAsync() => await CloudSaveService.Instance.Data.RetrieveAllKeysAsync();

        [Obsolete("The interface provided by SaveData.ForceSaveAsync(Dictionary<string, object>) has been replaced by CloudSaveService.Instance.Data.ForceSaveAsync(Dictionary<string, object>), and should be accessed from there instead. This API will be removed in an upcoming release.", false)]
        public static async Task ForceSaveAsync(Dictionary<string, object> data) => await CloudSaveService.Instance.Data.ForceSaveAsync(data);

        [Obsolete("The interface provided by SaveData.ForceDeleteAsync(string) has been replaced by CloudSaveService.Instance.Data.ForceDeleteAsync(string), and should be accessed from there instead. This API will be removed in an upcoming release.", false)]
        public static async Task ForceDeleteAsync(string key) => await CloudSaveService.Instance.Data.ForceDeleteAsync(key);

        [Obsolete("The interface provided by SaveData.LoadAsync(HashSet<string>) has been replaced by CloudSaveService.Instance.Data.LoadAsync(HashSet<string>), and should be accessed from there instead. This API will be removed in an upcoming release.", false)]
        public static async Task<Dictionary<string, string>> LoadAsync(HashSet<string> keys = null) => await CloudSaveService.Instance.Data.LoadAsync(keys);

        [Obsolete("The interface provided by SaveData.LoadAllAsync() has been replaced by CloudSaveService.Instance.Data.LoadAllAsync(), and should be accessed from there instead. This API will be removed in an upcoming release.", false)]
        public static async Task<Dictionary<string, string>> LoadAllAsync() => await CloudSaveService.Instance.Data.LoadAsync();
    }
}
