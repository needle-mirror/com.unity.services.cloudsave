using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
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

            // Data

            await ForceSaveSingleData("primitive_key", "value!");

            SampleObject outgoingSample = new SampleObject
            {
                AmazingFloat = 13.37f,
                SparklingInt = 1337,
                SophisticatedString = "hi there!"
            };
            await ForceSaveObjectData("object_key", outgoingSample);
            SampleObject incomingSample = await RetrieveSpecificData<SampleObject>("object_key");
            Debug.Log($"Loaded sample object: {incomingSample.AmazingFloat}, {incomingSample.SparklingInt}, {incomingSample.SophisticatedString}");

            await ForceDeleteSpecificData("object_key");
            await ListAllKeys();
            await RetrieveEverything();

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
                var keys = await CloudSaveService.Instance.Data.RetrieveAllKeysAsync();

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

                await CloudSaveService.Instance.Data.ForceSaveAsync(oneElement);

                Debug.Log($"Successfully saved {key}:{value}");
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

        private async Task ForceSaveObjectData(string key, SampleObject value)
        {
            try
            {
                // Although we are only saving a single value here, you can save multiple keys
                // and values in a single batch.
                Dictionary<string, object> oneElement = new Dictionary<string, object>
                {
                    { key, value }
                };

                await CloudSaveService.Instance.Data.ForceSaveAsync(oneElement);

                Debug.Log($"Successfully saved {key}:{value}");
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

        private async Task<T> RetrieveSpecificData<T>(string key)
        {
            try
            {
                var results = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> {key});

                if (results.TryGetValue(key, out string value))
                {
                    return JsonUtility.FromJson<T>(value);
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
                var results = await CloudSaveService.Instance.Data.LoadAllAsync();

                Debug.Log($"Elements loaded!");

                foreach (var element in results)
                {
                    Debug.Log($"Key: {element.Key}, Value: {element.Value}");
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
                await CloudSaveService.Instance.Data.ForceDeleteAsync(key);

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
                var results = await CloudSaveService.Instance.Files.ListAllAsync();

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
                var results = await CloudSaveService.Instance.Files.GetMetadataAsync(key);

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
                await CloudSaveService.Instance.Files.SaveAsync(key, bytes);

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
                await CloudSaveService.Instance.Files.SaveAsync(key, stream);

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
                var results = await CloudSaveService.Instance.Files.LoadBytesAsync(key);

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
                var results = await CloudSaveService.Instance.Files.LoadStreamAsync(key);

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
                await CloudSaveService.Instance.Files.DeleteAsync(key);

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
