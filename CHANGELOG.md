# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [2.0.1] - 2022-06-10
* Added missing XmlDoc to public ICloudSaveDataClient interface and CloudSaveService static class.

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
