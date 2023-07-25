namespace Unity.Services.CloudSave
{
    public class WriteLockOptions
    {
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
