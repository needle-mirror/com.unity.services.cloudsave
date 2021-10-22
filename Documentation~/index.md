# Cloud Save SDK Guide

This package helps you integrate the Cloud Save service into your game. 

## Getting Started

To get started with the Cloud Save SDK:

* Install the desired version of the package using Package Manager
* Sign in to your cloud project using the Services window in Unity
* Initialize the Core SDK using `await UnityServices.InitializeAsync()`
* Implement the Authentication sign in flow at a convenient point in your application.

**Note**: The Cloud Save SDK requires that an authentication flow from the Authentication SDK has been completed prior to using any of the Cloud Save APIs, as a valid player ID and access token are required to access the Cloud Save services. This can be achieved with the following code snippet for anonymous authentication, or see the documentation for the Authentication SDK for more details and other sign in methods:

```cs
await AuthenticationService.Instance.SignInAnonymouslyAsync();
```

## Using the SDK

### Initial Setup

The Cloud Save SDK is ready to use immediately once sign in with the Authentication SDK is complete. You may then call any of the below methods to start interacting with the Cloud Save data.

### Quick Reference

Below is a quick reference for the methods available in the SDK.

The methods are:
```cs
Task<List<string>> RetrieveAllKeysAsync();
Task ForceSaveAsync(Dictionary<string, object> data);
Task ForceDeleteAsync(string key);
Task<Dictionary<string, strinf>> LoadAsync(HashSet<string> keys);
Task<Dictionary<string, object>> LoadAllAsync();
```

Every method includes very basic client validation that checks whether the player ID, project ID or access token are missing before making any API call.

#### RetrieveAllKeysAsync

Developers can retrieve all keys that are stored in Cloud Save for a player.
This method includes pagination.

#### ForceSaveAsync

Developers can force upload one or more key-value pairs to the Cloud Save - it ignores write lock validation.
`Dictionary` as a parameter ensures the uniqueness of given keys.

#### ForceDeleteAsync

Developers can remove one key at the time. If a given key doesn't exist, there is no feedback in place to inform a developer about it. 
There is no write lock implemented for this method.

#### LoadAsync

Developers can download one or more key-values from Cloud Save, based on provided keys. `HashSet` as a parameter ensures the uniqueness of keys. 
This method includes pagination.

#### LoadAllAsync

Developers can download all key-values from the Cloud Save service. 
This method includes pagination.

### Errors

#### CloudSaveException

CloudSaveExceptions maps:
- exceptions that might be returned from the API
- client validation errors that might occur (missing either project ID, player ID or access token).

Apart from fields that are normally provided by C# Exception, CloudSaveException includes a reason (enum `CloudSaveExceptionReason`) and an error code that can help with debugging and finding out in the documentation what went wrong.
A message contains a user-friendly explanation of what went wrong.

A CloudSaveExceptionReason is an enum value that describes what category of issue occurred. This is provided to allow a code-friendly way of detecting and handling the different types of errors that can be thrown. The possible values are:

**ProjectIdMissing**: Project ID is missing - make sure the project is correctly linked to your game and try again.

**PlayerIdMissing**: Player ID is missing - ensure you are signed in through the Authentication SDK and try again.

**AccessTokenMissing**: Access token is missing - ensure you are signed in through the Authentication SDK and try again.

**InvalidArgument**: One of the parameters was missing or invalid. It might indicate problems around API-based validation. Check the documentation for the latest key/value requirements and ensure they are met.

**Unauthorized**: The provided auth token is invalid. In most cases in the Cloud Save SDK, this means that the Authentication SDK sign in process has not yet completed, or has expired. Ensure that SDK is signed in correctly before calling any Cloud Save SDK methods.

**KeyLimitExceeded**: Key-value pair limit per user has been exceeded. Pushing a new key-value pair will require a removal of an already existing one on the server.

**NotFound**: The action that was requested could not be completed as the specified resource is not found. This is the error that might be thrown either by API or a client-based validation. Check if the correct project ID was linked to the game, signing in process has been completed or a passed argument is correct.

**ServiceUnavailable**: The Cloud Save service is currently unavailable.

**TooManyRequests**: Too many requests have been sent in a short period of time, which resulted in a device being rate limited. This usually indicates a logic problem in the calling code, so check the logic around the offending method call.

**Unknown**: An error was returned that wasn't expected by the SDK. This is often a bug, and should be raised with the SDK team to debug.

#### CloudSaveValidationException

This exception derives from `CloudSaveException`, but it gives more insight to validation isues that came up after reaching the service.

This class includes extra field: `List<CloudSaveValidationErrorDetail> Details`, where `CloudSaveValidationErrorDetail` is a representation of the single error from the API's Validation Error Response.