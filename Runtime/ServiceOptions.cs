namespace Unity.Services.CloudSave
{
    /// <summary>
    /// Options for storing writelock
    /// </summary>
    public class WriteLockOptions
    {
        /// <summary>
        /// The writelock
        /// </summary>
        public string WriteLock { get; set; }
    }

    /// <summary>
    /// Options for save operations
    /// </summary>
    public class SaveOptions : WriteLockOptions
    {
    }

    /// <summary>
    /// Options for delete operations
    /// </summary>
    public class DeleteOptions : WriteLockOptions
    {
    }
}
