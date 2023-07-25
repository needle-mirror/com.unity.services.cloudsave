using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Models;
using Unity.Services.Core;
using UnityEngine;

namespace CloudSaveSample
{
    [Serializable]
    public class SampleObject
    {
        public string SophisticatedString;
        public int SparklingInt;
        public float AmazingFloat;
    }

    public class CloudSaveSample : MonoBehaviour
    {
        private async void Awake()
        {
            // Cloud Save needs to be initialized along with the other Unity Services that
            // it depends on (namely, Authentication), and then the user must sign in.
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

            Debug.Log("Signed in?");

            // Player Data

            // first saves without write lock
            await ForceSaveSingleData("primitive_key", "value!");

            SampleObject firstSample = new SampleObject
            {
                AmazingFloat = 13.37f,
                SparklingInt = 1337,
                SophisticatedString = "hi there!"
            };
            string writeLock = await ForceSaveObjectData("object_key", firstSample);
            SampleObject incomingSample = await RetrieveSpecificData<SampleObject>("object_key");
            Debug.Log($"Loaded sample object: {incomingSample.AmazingFloat}, {incomingSample.SparklingInt}, {incomingSample.SophisticatedString}, write lock {writeLock}");

            // second save with write lock
            SampleObject secondSample = new SampleObject
            {
                AmazingFloat = 42.26f,
                SparklingInt = 4226,
                SophisticatedString = "hi there... again!"
            };
            string updatedWriteLock = await SaveObjectData("object_key", secondSample, writeLock);
            SampleObject updatedSample = await RetrieveSpecificData<SampleObject>("object_key");
            Debug.Log($"Loaded updated sample object: {updatedSample.AmazingFloat}, {updatedSample.SparklingInt}, {updatedSample.SophisticatedString}, write lock {updatedWriteLock}");

            // deletion with wrong write lock, this will fail with a validation error
            await DeleteSpecificData("object_key", "incorrect-write-lock");

            // force delete without write lock
            await ForceDeleteSpecificData("object_key");

            await ListAllKeys();
            await RetrieveEverything();

            // Custom Data, read-only

            await ListAllCustomKeys("custom-test-id");
            await RetrieveAllCustomData("custom-test-id");

            // Files

            var inputBytes = Encoding.UTF8.GetBytes("test content for file bytes");
            var inputStream = new MemoryStream(Encoding.UTF8.GetBytes("test content for file stream"));

            await SaveFileBytes("bytes-test-file", inputBytes);
            await SaveFileStream("stream-test-file", inputStream);

            await ListAllFiles();
            await GetFileMetadata("bytes-test-file");
            await GetFileMetadata("stream-test-file");

            var fileBytes = await LoadFileBytes("bytes-test-file");
            Debug.Log($"Loaded sample file containing content: {Encoding.UTF8.GetString(fileBytes)}");

            using var fileStream = await LoadFileStream("stream-test-file");
            using var streamReader = new StreamReader(fileStream);
            Debug.Log($"Loaded sample file containing content: {await streamReader.ReadToEndAsync()}");

            await DeleteFile("bytes-test-file");
            await DeleteFile("stream-test-file");
        }

        private async Task ListAllKeys()
        {
            try
            {
                var keys = await CloudSaveService.Instance.Data.Player.ListAllKeysAsync();

                Debug.Log($"Keys count: {keys.Count}\n" +
                    $"Keys: {String.Join(", ", keys)}");
            }
            catch (CloudSaveValidationException e)
            {
                Debug.LogError(e);
            }
            catch (CloudSaveRateLimitedException e)
            {
                Debug.LogError(e);
            }
            catch (CloudSaveException e)
            {
                Debug.LogError(e);
            }
        }

        private async Task ListAllCustomKeys(string customId)
        {
            try
            {
                var keys = await CloudSaveService.Instance.Data.Custom.ListAllKeysAsync(customId);

                Debug.Log($"Keys count for custom ID {customId}: {keys.Count}\n" +
                    $"Keys: {String.Join(", ", keys)}");
            }
            catch (CloudSaveValidationException e)
            {
                Debug.LogError(e);
            }
            catch (CloudSaveRateLimitedException e)
            {
                Debug.LogError(e);
            }
            catch (CloudSaveException e)
            {
                Debug.LogError(e);
            }
        }

        private async Task ForceSaveSingleData(string key, string value)
        {
            try
            {
                Dictionary<string, object> oneElement = new Dictionary<string, object>();

                // It's a text input field, but let's see if you actually entered a number.
                if (Int32.TryParse(value, out int wholeNumber))
                {
                    oneElement.Add(key, wholeNumber);
                }
                else if (Single.TryParse(value, out float fractionalNumber))
                {
                    oneElement.Add(key, fractionalNumber);
                }
                else
                {
                    oneElement.Add(key, value);
                }

                // Saving the data without write lock validation by passing the data as an object instead of a SaveItem
                Dictionary<string, string> result = await CloudSaveService.Instance.Data.Player.SaveAsync(oneElement);

                Debug.Log($"Successfully saved {key}:{value} with updated write lock {result[key]}");
            }
            catch (CloudSaveValidationException e)
            {
                Debug.LogError(e);
            }
            catch (CloudSaveRateLimitedException e)
            {
                Debug.LogError(e);
            }
            catch (CloudSaveException e)
            {
                Debug.LogError(e);
            }
        }

        private async Task<string> ForceSaveObjectData(string key, SampleObject value)
        {
            try
            {
                // Although we are only saving a single value here, you can save multiple keys
                // and values in a single batch.
                Dictionary<string, object> oneElement = new Dictionary<string, object>
                {
                    { key, value }
                };

                // Saving data without write lock validation by passing the data as an object instead of a SaveItem
                Dictionary<string, string> result = await CloudSaveService.Instance.Data.Player.SaveAsync(oneElement);
                string writeLock = result[key];

                Debug.Log($"Successfully saved {key}:{value} with updated write lock {writeLock}");

                return writeLock;
            }
            catch (CloudSaveValidationException e)
            {
                Debug.LogError(e);
            }
            catch (CloudSaveRateLimitedException e)
            {
                Debug.LogError(e);
            }
            catch (CloudSaveException e)
            {
                Debug.LogError(e);
            }

            return null;
        }

        private async Task<string> SaveObjectData(string key, SampleObject value, string writeLock)
        {
            try
            {
                // Although we are only saving a single value here, you can save multiple keys
                // and values in a single batch.
                // Use SaveItem to specify a write lock. The request will fail if the provided write lock
                // does not match the one currently saved on the server.
                Dictionary<string, SaveItem> oneElement = new Dictionary<string, SaveItem>
                {
                    { key, new SaveItem(value, writeLock) }
                };

                // Saving data with write lock validation by using a SaveItem with the write lock specified
                Dictionary<string, string> result = await CloudSaveService.Instance.Data.Player.SaveAsync(oneElement);
                string newWriteLock = result[key];

                Debug.Log($"Successfully saved {key}:{value} with updated write lock {newWriteLock}");

                return newWriteLock;
            }
            catch (CloudSaveValidationException e)
            {
                Debug.LogError(e);
            }
            catch (CloudSaveRateLimitedException e)
            {
                Debug.LogError(e);
            }
            catch (CloudSaveException e)
            {
                Debug.LogError(e);
            }

            return null;
        }

        private async Task<T> RetrieveSpecificData<T>(string key)
        {
            try
            {
                var results = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> {key});

                if (results.TryGetValue(key, out var item))
                {
                    return item.Value.GetAs<T>();
                }
                else
                {
                    Debug.Log($"There is no such key as {key}!");
                }
            }
            catch (CloudSaveValidationException e)
            {
                Debug.LogError(e);
            }
            catch (CloudSaveRateLimitedException e)
            {
                Debug.LogError(e);
            }
            catch (CloudSaveException e)
            {
                Debug.LogError(e);
            }

            return default;
        }

        private async Task RetrieveEverything()
        {
            try
            {
                // If you wish to load only a subset of keys rather than everything, you
                // can call a method LoadAsync and pass a HashSet of keys into it.
                var results = await CloudSaveService.Instance.Data.Player.LoadAllAsync();

                Debug.Log($"{results.Count} elements loaded!");

                foreach (var result in results)
                {
                    Debug.Log($"Key: {result.Key}, Value: {result.Value.Value}");
                }
            }
            catch (CloudSaveValidationException e)
            {
                Debug.LogError(e);
            }
            catch (CloudSaveRateLimitedException e)
            {
                Debug.LogError(e);
            }
            catch (CloudSaveException e)
            {
                Debug.LogError(e);
            }
        }

        private async Task RetrieveAllCustomData(string customId)
        {
            try
            {
                // If you wish to load only a subset of keys rather than everything, you
                // can call a method LoadAsync and pass a HashSet of keys into it.
                var results = await CloudSaveService.Instance.Data.Custom.LoadAllAsync(customId);

                Debug.Log($"{results.Count} elements loaded from custom Id {customId}!");

                foreach (var result in results)
                {
                    Debug.Log($"Key: {result.Key}, Value: {result.Value.Value}");
                }
            }
            catch (CloudSaveValidationException e)
            {
                Debug.LogError(e);
            }
            catch (CloudSaveRateLimitedException e)
            {
                Debug.LogError(e);
            }
            catch (CloudSaveException e)
            {
                Debug.LogError(e);
            }
        }

        private async Task ForceDeleteSpecificData(string key)
        {
            try
            {
                // Deletion of the key without write lock validation by omitting the DeleteOptions argument
                await CloudSaveService.Instance.Data.Player.DeleteAsync(key);

                Debug.Log($"Successfully deleted {key}");
            }
            catch (CloudSaveValidationException e)
            {
                Debug.LogError(e);
            }
            catch (CloudSaveRateLimitedException e)
            {
                Debug.LogError(e);
            }
            catch (CloudSaveException e)
            {
                Debug.LogError(e);
            }
        }

        private async Task DeleteSpecificData(string key, string writeLock)
        {
            try
            {
                // Deletion of the key with write lock validation
                await CloudSaveService.Instance.Data.Player.DeleteAsync(key, new DeleteOptions{ WriteLock = writeLock});

                Debug.Log($"Successfully deleted {key}");
            }
            catch (CloudSaveValidationException e)
            {
                Debug.LogError(e);
            }
            catch (CloudSaveRateLimitedException e)
            {
                Debug.LogError(e);
            }
            catch (CloudSaveException e)
            {
                Debug.LogError(e);
            }
        }

        private async Task ListAllFiles()
        {
            try
            {
                var results = await CloudSaveService.Instance.Files.Player.ListAllAsync();

                Debug.Log("Metadata loaded for all files!");

                foreach (var element in results)
                {
                    Debug.Log($"Key: {element.Key}, File Size: {element.Size}");
                }
            }
            catch (CloudSaveValidationException e)
            {
                Debug.LogError(e);
            }
            catch (CloudSaveRateLimitedException e)
            {
                Debug.LogError(e);
            }
            catch (CloudSaveException e)
            {
                Debug.LogError(e);
            }
        }

        private async Task GetFileMetadata(string key)
        {
            try
            {
                var results = await CloudSaveService.Instance.Files.Player.GetMetadataAsync(key);

                Debug.Log("File metadata loaded!");

                Debug.Log($"Key: {results.Key}, File Size: {results.Size}");
            }
            catch (CloudSaveValidationException e)
            {
                Debug.LogError(e);
            }
            catch (CloudSaveRateLimitedException e)
            {
                Debug.LogError(e);
            }
            catch (CloudSaveException e)
            {
                Debug.LogError(e);
            }
        }

        private async Task SaveFileBytes(string key, byte[] bytes)
        {
            try
            {
                await CloudSaveService.Instance.Files.Player.SaveAsync(key, bytes);

                Debug.Log("File saved!");
            }
            catch (CloudSaveValidationException e)
            {
                Debug.LogError(e);
            }
            catch (CloudSaveRateLimitedException e)
            {
                Debug.LogError(e);
            }
            catch (CloudSaveException e)
            {
                Debug.LogError(e);
            }
        }

        private async Task SaveFileStream(string key, Stream stream)
        {
            try
            {
                await CloudSaveService.Instance.Files.Player.SaveAsync(key, stream);

                Debug.Log("File saved!");
            }
            catch (CloudSaveValidationException e)
            {
                Debug.LogError(e);
            }
            catch (CloudSaveRateLimitedException e)
            {
                Debug.LogError(e);
            }
            catch (CloudSaveException e)
            {
                Debug.LogError(e);
            }
        }

        private async Task<byte[]> LoadFileBytes(string key)
        {
            try
            {
                var results = await CloudSaveService.Instance.Files.Player.LoadBytesAsync(key);

                Debug.Log("File loaded!");

                return results;
            }
            catch (CloudSaveValidationException e)
            {
                Debug.LogError(e);
            }
            catch (CloudSaveRateLimitedException e)
            {
                Debug.LogError(e);
            }
            catch (CloudSaveException e)
            {
                Debug.LogError(e);
            }

            return null;
        }

        private async Task<Stream> LoadFileStream(string key)
        {
            try
            {
                var results = await CloudSaveService.Instance.Files.Player.LoadStreamAsync(key);

                Debug.Log("File loaded!");

                return results;
            }
            catch (CloudSaveValidationException e)
            {
                Debug.LogError(e);
            }
            catch (CloudSaveRateLimitedException e)
            {
                Debug.LogError(e);
            }
            catch (CloudSaveException e)
            {
                Debug.LogError(e);
            }

            return null;
        }

        private async Task DeleteFile(string key)
        {
            try
            {
                await CloudSaveService.Instance.Files.Player.DeleteAsync(key);

                Debug.Log("File deleted!");
            }
            catch (CloudSaveValidationException e)
            {
                Debug.LogError(e);
            }
            catch (CloudSaveRateLimitedException e)
            {
                Debug.LogError(e);
            }
            catch (CloudSaveException e)
            {
                Debug.LogError(e);
            }
        }
    }
}
