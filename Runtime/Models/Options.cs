namespace Unity.Services.CloudSave.Models
{
    public abstract class WriteLockOptions
    {
        public string WriteLock { get; set; }
    }

    public interface IAccessControlOptions
    {
        public IAccessClassOptions AccessClassOptions { get; }
    }

    public interface IAccessClassOptions
    {
        public AccessClass AccessClass { get; }
        public string PlayerId { get; }
    }

    public enum AccessClass
    {
        Default,
        Private,
        Protected,
        Public
    }
}
