# Installation

## Getting Started

To get started with the Cloud Save SDK:

* Ensure the service has been enabled via the cloud save service dashboard page
* Install the desired version of the package using Package Manager
* Sign in to your cloud project using the `Edit > Project Settings > Services` window within the Unity Editor
* Initialize the Core SDK using `await UnityServices.InitializeAsync()`
* Sign in via the Authentication SDK, as mentioned below

**Note**: The Cloud Save SDK requires that an authentication flow from the Authentication SDK has been completed prior to using any of the Cloud Save APIs, as a valid player ID and access token are required to access the Cloud Save services. This can be achieved with the following code snippet for anonymous authentication, or see the documentation for the Authentication SDK for more details and other sign in methods:

```cs
await AuthenticationService.Instance.SignInAnonymouslyAsync();
```

## Using the SDK

The Cloud Save SDK is ready to use once you have signed in with the Authentication SDK. You can then call any of the Cloud Save methods to start interacting with your data.
