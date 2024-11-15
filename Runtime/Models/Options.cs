namespace Unity.Services.CloudSave.Models
{
    /// <summary>
    /// Option class for write locks.
    /// </summary>
    public abstract class WriteLockOptions
    {
        /// <summary>
        /// Write lock.
        /// </summary>
        public string WriteLock { get; set; }
    }

    /// <summary>
    /// Option class for access control.
    /// </summary>
    public interface IAccessControlOptions
    {
        /// <summary>
        /// Option class for access classes.
        /// </summary>
        public IAccessClassOptions AccessClassOptions { get; }
    }

    /// <summary>
    /// Option class for access classes.
    /// </summary>
    public interface IAccessClassOptions
    {
        /// <summary>
        /// Access class for the request.
        /// </summary>
        public AccessClass AccessClass { get; }

        /// <summary>
        /// Player ID to make the request for.
        /// </summary>
        public string PlayerId { get; }
    }

    /// <summary>
    /// Enum to represent possible access classes for a request.
    /// </summary>
    public enum AccessClass
    {
        /// <summary>
        /// Default access class.
        /// </summary>
        Default,

        /// <summary>
        /// Private access class.
        /// </summary>
        Private,

        /// <summary>
        /// Protected access class.
        /// </summary>
        Protected,

        /// <summary>
        /// Public access class.
        /// </summary>
        Public
    }
}
