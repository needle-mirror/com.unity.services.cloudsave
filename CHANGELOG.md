# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).
## [3.1.1] - 2024-02-08
* Added Apple Privacy Manifest

## [3.1.0] - 2023-11-23
### Changed
* The existing `CloudSaveService.Instance.Data.Player.DeleteAsync` has been marked as Obsolete, with a new version added that accepts options of type `CloudSave.Models.Data.Player.DeleteOptions` instead of `CloudSave.DeleteOptions`.
  This enables the addition of new options to support Access Classes (see below in **Added**).

### Added
* Support for Access Classes when interacting with Player Data, via the addition of optional `options` objects to existing methods in the `CloudSaveService.Instance.Data.Player` API.
For more information on Access Classes, please refer to [the documentation](https://docs.unity.com/ugs/en-us/manual/cloud-save/manual/concepts/player-data#Access_Classes).
  * Allows players to save data to the Public Access Class in addition to the existing Default, which allows saved data to be visible to other players.
  * Allows players to read data from Public and Protected Access Classes in addition to the existing Default, where Protected Player Data can only be set by a server authoritative source (e.g. Cloud Code)
  * Allows players to read other players' Public Player Data, by providing their Player ID as part of the new `options` object.

* Support for Querying in both Public Player Data and Default Custom Data, via the new `QueryAsync` method.
For more information on Querying, please refer to [the documentation](https://docs.unity.com/ugs/en-us/manual/cloud-save/manual/concepts/queries).
  * Any data for which an index is configured can be queried by specifying filters on the indexed data (e.g. clanMemberCount < 20)
  * Any data stored for the returned entities (whether indexed or not) can be retrieved as part of the query response by specifying `ReturnKeys` in the `Query` object.

## [3.0.0] - 2023-07-25
### Changed
* All existing methods have been marked as obsolete. They have mostly been replicated in new namespaces with some additional changes:
  * Methods in the namespace `CloudSaveService.Instance.Files` have been replicated in the namespace `CloudSaveService.Instance.Files.Player` without additional changes.
  * Methods in the namespace `CloudSaveService.Instance.Data` have been replicated in the namespace `CloudSaveService.Instance.Data.Player` with some additional changes:
    * `RetrieveAllKeysAsync` has been renamed to `ListAllKeysAsync` and now returns a `Task<List<ItemKey>>` where `ItemKey` includes additional metadata alongside the key: The write lock value, and modified date-time.
    * `LoadAsync` and `LoadAllAsync` now return a `Task<Dictionary<string, Item>>`, where `Item` includes additional metadata alongside the value: The write lock value, modified date-time, and created date-time.
    * `ForceSaveAsync` has been removed in favour of `SaveAsync(IDictionary<string, object> data)`, or `SaveAsync(IDictionary<string, SaveItem> data)` without specifying the write lock on the `SaveItem`.
    * `ForceDeleteAsync` has been removed in favour of `DeleteAsync` without specifying the write lock option.

### Added
* Support for reading data from custom IDs stored with Game State for Cloud Save Data. This data is read-only from the SDK and the following methods are available from the namespace `CloudSaveService.Instance.Data.Custom.*`
  * `ListAllKeysAsync` will list all keys for a given custom data ID with their metadata
  * `LoadAsync` will load the data for the specified keys for a given custom data ID
  * `LoadAllAsync` will load all the data for a given custom data ID
* Support for reading and enforcing write locks on player data writes:
  * Added `SaveAsync(IDictionary<string, SaveItem> data)` which will fail for the given item if the supplied write lock on the `SaveItem` does not match the server state
  * `SaveAsync` returns a `Dictionary<string, string>` with the saved keys and their updated write locks
  * Added `DeleteAsync` with an optional `DeleteOptions` parameter which will fail if the specified write lock option does not match the server state

## [2.2.1] - 2023-04-27
* `ForceSaveAsync` now supports batching when trying to save more than 20 keys in a single call
* Cloud Save Files support, including write lock support for all appropriate methods.
  * `ListAllAsync` lists all files belonging to the signed in player with metadata
  * `GetMetadataAsync` returns the metadata for a given file
  * `SaveAsync` will upload a given file to Cloud Save Files storage for the player, supports either a Stream or a byte[]
  * `LoadStreamAsync` will download a given file from Cloud Save Files storage for the player, and returns a Stream object containing the file data
  * `LoadBytesAsync` will download a given file from Cloud Save Files storage for the player, and returns a byte[] object containing the file data

## [2.0.1] - 2022-06-10
* Added missing XmlDoc to public `ICloudSaveDataClient` interface and `CloudSaveService` static class.

## [2.0.0] - 2022-05-12
* The Cloud Save SDK is no longer pre-release!

## [2.0.0-pre.2] - 2022-05-06

* **Breaking Change:** Code in the `Unity.Services.CloudSave.Editor.Settings` namespace has been made internal as it was never meant to be public.
* Updated dependencies.

## [2.0.0-pre.1] - 2022-03-14

* Added mechanism to halt web traffic when request limits have been exceeded and requests are guaranteed to be rejected by the server.
* The Cloud Save service is now accessed using `CloudSaveService.Instance.Data.<API>`.
* When a rate limit error occurs, a specific `CloudSaveRateLimitedException` will now be thrown. The new exception type includes the RetryAfter value (in seconds).
* Added the Project Settings tab with link to Cloud Save dashboard.

## [1.0.0-pre.3] - 2021-10-14

* All models that weren't documented have been made internal as they were not designed to be used externally.
* Improved documentation (sample scene and annotations).
* Updated dependencies (Core and Authentication).

## [1.0.0] - 2021-08-17

* Open Beta release
* Updated dependencies (Core and Authentication).
* Methods marked with `Obsolete` annotations have been removed.
* `LoadAsync` has been split into two separate methods: `LoadAsyncAll` and `LoadAsync(HashSet<string> keys)`. 
* Both Load-related methods now return `Dictionary<string, string>`, with a JSON serialized value that needs to be deserialized by the user.
* Package-specific error types are now: `CloudSaveException` and `CloudSaveValidationException` (More properties are now available for debugging the issues).
* Removed Moq dependency from the package.
* CloudSaveSample was transformed into the code-example.

## [0.5.0-preview] - 2021-07-30

 * Core SDK has been updated to `v.1.1.0-pre.5`.
 * Authentication package version is now on version `1.0.0-pre.1`.
 * Internals were updated to use the latest REST APIs from the Cloud Save Service for the long-term resiliency.
 * Interface methods have been renamed to be in sync with the Unity naming convention. All public async functions now include `Async` suffix. Old methods are still available, but with `Obsolete` annotation. They will be removed as a part of the next release.

## [0.4.0-preview] - 2021-06-17

* All dependencies are now up to date:
** Core SDK has been updated to v.1.1.0-pre.2
** Authentication package version is now 0.5.0-preview
** Code-gen API wrapper updated to v.0.2.0

### Bug fixes
* The latest API changes are now addressed - all interface methods work again (no breaking changes have been introduced).

### Known Issues
* There is an issue with the sample scene where links between elements and objects in code are missing.

## [0.3.0-preview] - 2021-05-27

### New Features

* Very basic client-based validation is now in place to lower the number of API calls that would result in errors.
* Sample scene has been improved to be more user-friendly.

### Bug fixes
* All methods are now functioning correctly on iOS.
* No more intermittent "Unknown" exceptions while loading all data from the server.

## [0.2.0-preview] - 2021-05-24

### New Features

* Core SDK integration - Cloud Save supports the Core initialisation and authentication flows.
* Exceptions are now more user-friendly - methods will throw consistent exceptions.
* Code-gen API wrapper updated to v0.26.0.

### Bug fixes
* Removed console errors for conflicting meta files between dependencies.
* Improved pagination.

### Known Issues
* Intermittent "Unknown" exceptions coming from codegen while trying to load all data from the server.
* Currently unavailable for iOS - all actions throwing errors. 

## [0.1.0-preview] - 2021-05-14

This is the initial release of the Cloud Save SDK.

### New Features

* Retrieve all keys from CloudSave for the currently signed in player.
* Load player's key-value pairs (all or specified keys) from CloudSave.
* Save up to 20 key-value pairs at once (max 200 in total) for a player.
* Delete one key-value pair based on provided key.

### Known Issues

* Every exception is returned as BasicError (generic error wrapper).
* There is no client-side validation.
