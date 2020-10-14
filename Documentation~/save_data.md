# SaveData

The methods in the SaveData namespace allow you to interact with your data.

Every method includes basic client validation that checks whether the player ID, project ID or access token are missing before making any API call.

## RetrieveAllKeysAsync

Retrieves all keys that are stored in Cloud Save for a player.
This method includes pagination.

## ForceSaveAsync

Force uploads one or more key-value pairs to the Cloud Save. This will overwrite any values currently held under those keys.
Takes a `Dictionary` as a parameter. This ensures the uniqueness of given keys.

## ForceDeleteAsync

Force removes one key at the time.
Trying to delete a key that doesn't exist will not return anything.

## LoadAsync

Retrieves one or more key-values from Cloud Save, based on provided keys. 
Takes a `HashSet` as a parameter. This ensures the uniqueness of keys. 
This method includes pagination.

## LoadAllAsync

Retrieves all key-values from the Cloud Save service. 
This method includes pagination.