# Exceptions

## CloudSaveException

`CloudSaveException` maps to:
- Exceptions that are returned from the API
- Client validation errors that might occur (missing Project ID, Player ID or Access Token).

A `CloudSaveException` contains:
- All the fields normally provided by a C# Exception.
- An enum reason (enum `CloudSaveExceptionReason`) and an error code.
- A user-friendly message that contains an explanation of what went wrong.

### CloudSaveExceptionReason
A `CloudSaveExceptionReason` is an enum value that describes what category of issue occurred. This is provided to allow a code-friendly way of detecting and handling the different types of errors that can be thrown. 

The possible values are:

- **ProjectIdMissing**: Project ID is missing - ensure your cloud project is correctly linked.

- **PlayerIdMissing**: Player ID is missing - ensure you are signed in through the Authentication SDK.

- **AccessTokenMissing**: Access token is missing - ensure you are signed in through the Authentication SDK.

- **InvalidArgument**: One of the parameters was missing or invalid. This may indicate problems around API-based validation. Check the documentation for the latest key/value requirements and ensure they are met.

- **Unauthorized**: The provided auth token is invalid. In most cases, this means that the Authentication SDK sign in process has not yet completed, or has expired. Ensure that SDK is signed in correctly before calling any Cloud Save SDK methods.

- **KeyLimitExceeded**: Key-value pair limit per user has been exceeded. Pushing a new key-value pair will require a removal of an already existing one on the server.

- **NotFound**: The action that was requested could not be completed as the specified resource is not found. This might be thrown either by API or a client-based validation. Check if the correct project ID is linked to your game, the signing in process has been completed and the passed argument is correct.

- **ServiceUnavailable**: The Cloud Save service is currently unavailable.

- **TooManyRequests**: Too many requests have been sent in a short period of time, which resulted in a device being rate limited. This usually indicates a logic problem in the calling code, so check the logic around the offending method call.

- **Unknown**: An error was returned that wasn't expected by the SDK.

## CloudSaveValidationException

This exception inherits from `CloudSaveException` and gives more insight into the validation issues that came up after reaching the service.

It includes one extra field: 
- **Details**: This is a list of errors returned from the API's Validation Error Response. It is a list of `CloudSaveValidationErrorDetail` (see below).

### CloudSaveValidationErrorDetail

Represents a single error returned from the API's Validation Error Response.

It contains three fields:

- **Field**: The field in the data that caused the error. This is a string.
- **Key**: The data key that caused the error. This is a string.
- **Messages**: Messages that describe the errors. This is a list of strings.
