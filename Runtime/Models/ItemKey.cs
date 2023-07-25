using System;

namespace Unity.Services.CloudSave.Models
{
    /// <summary>
    /// Response type for a data key stored in the Cloud Save service with its metadata.
    /// </summary>
    public class ItemKey
    {
        /// <summary>
        /// Response type for a Data Item stored in the Cloud Save service.
        /// </summary>
        /// <param name="key">The data key</param>
        /// <param name="writeLock">Write lock value for the key, to be used for enforcing conflict checking when updating an existing data item</param>
        /// <param name="modified">The datetime when the value was last modified</param>
        internal ItemKey(string key, string writeLock, DateTime? modified)
        {
            Key = key;
            WriteLock = writeLock;
            Modified = modified;
        }

        /// <summary>
        /// The data key
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// The write lock value for the data, used for enforcing conflict checking when updating an existing data item
        /// </summary>
        public string WriteLock { get; }

        /// <summary>
        /// The datetime when the value was last modified
        /// </summary>
        public DateTime? Modified { get; }
    }
}
