using Unity.Services.CloudSave.Internal.Http;

namespace Unity.Services.CloudSave
{
    /// <summary>
    /// Response type for a Data Item stored in the Cloud Save service.
    /// </summary>
    public class Item
    {
        /// <summary>
        /// Response type for a Data Item stored in the Cloud Save service.
        /// </summary>
        /// <param name="value">Any JSON serializable structure</param>
        /// <param name="writeLock">Enforces conflict checking when updating an existing data item. This field should be omitted when creating a new data item. When updating an existing item, omitting this field ignores write conflicts. When present, an error response will be returned if the writeLock in the request does not match the stored writeLock.</param>
        public Item(IDeserializable value, string writeLock)
        {
            Value = value;
            WriteLock = writeLock;
        }

        /// <summary>
        /// Any JSON serializable structure
        /// </summary>
        public IDeserializable Value { get; }

        /// <summary>
        /// Enforces conflict checking when updating an existing data item. This field should be omitted when creating a new data item. When updating an existing item, omitting this field ignores write conflicts. When present, an error response will be returned if the writeLock in the request does not match the stored writeLock.
        /// </summary>
        public string WriteLock { get; }
    }
}
