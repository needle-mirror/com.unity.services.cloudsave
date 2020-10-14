# Quick Reference

Below is a quick reference for the methods available in the SDK.

## SaveData

The methods in the SaveData namespace allow you to interact with your data.

For more details on these methods, see [the SaveData documentation here.](./save_data.md)

The available methods are:
```cs
Task<List<string>> RetrieveAllKeysAsync();
Task ForceSaveAsync(Dictionary<string, object> data);
Task ForceDeleteAsync(string key);
Task<Dictionary<string, string>> LoadAsync(HashSet<string> keys);
Task<Dictionary<string, object>> LoadAllAsync();
```