namespace Unity.Services.CloudSave.Models.Data.Player
{
    /// <summary>
    /// Options for deleting data.
    /// </summary>
    public class DeleteOptions : WriteLockOptions, IAccessControlOptions
    {
        /// <summary>
        /// Access class for the operation.
        /// </summary>
        public IAccessClassOptions AccessClassOptions { get; }

        /// <summary>
        /// Options for deleting data.
        /// </summary>
        public DeleteOptions()
        {
            AccessClassOptions = new DefaultWriteAccessClassOptions();
        }

        /// <summary>
        /// Options for deleting data.
        /// </summary>
        /// <param name="accessClassOptions">Access class for the operation.</param>
        public DeleteOptions(WriteAccessClassOptions accessClassOptions)
        {
            AccessClassOptions = accessClassOptions ?? new DefaultWriteAccessClassOptions();
        }
    }

    /// <summary>
    /// Options for deleting multiple data keys.
    /// </summary>
    public class DeleteAllOptions : IAccessControlOptions
    {
        /// <summary>
        /// Access class for the operation.
        /// </summary>
        public IAccessClassOptions AccessClassOptions { get; }

        /// <summary>
        /// Options for deleting multiple data keys.
        /// </summary>
        public DeleteAllOptions()
        {
            AccessClassOptions = new DefaultWriteAccessClassOptions();
        }

        /// <summary>
        /// Options for deleting multiple data keys.
        /// </summary>
        /// <param name="accessClassOptions">Access class for the operation.</param>
        public DeleteAllOptions(WriteAccessClassOptions accessClassOptions)
        {
            AccessClassOptions = accessClassOptions ?? new DefaultWriteAccessClassOptions();
        }
    }

    /// <summary>
    /// Options for saving data.
    /// </summary>
    public class SaveOptions : IAccessControlOptions
    {
        /// <summary>
        /// Access class for the operation.
        /// </summary>
        public IAccessClassOptions AccessClassOptions { get; }

        /// <summary>
        /// Options for saving data.
        /// </summary>
        public SaveOptions()
        {
            AccessClassOptions = new DefaultWriteAccessClassOptions();
        }

        /// <summary>
        /// Options for saving data.
        /// </summary>
        /// <param name="accessClassOptions">Access class for the operation.</param>
        public SaveOptions(WriteAccessClassOptions accessClassOptions)
        {
            AccessClassOptions = accessClassOptions ?? new DefaultWriteAccessClassOptions();
        }
    }

    /// <summary>
    /// Options for listing data keys.
    /// </summary>
    public class ListAllKeysOptions : IAccessControlOptions
    {
        /// <summary>
        /// Access class for the operation.
        /// </summary>
        public IAccessClassOptions AccessClassOptions { get; }

        /// <summary>
        /// Options for listing data keys.
        /// </summary>
        public ListAllKeysOptions()
        {
            AccessClassOptions = new DefaultReadAccessClassOptions();
        }

        /// <summary>
        /// Options for listing data keys.
        /// </summary>
        /// <param name="accessClassOptions">Access class for the operation.</param>
        public ListAllKeysOptions(ReadAccessClassOptions accessClassOptions)
        {
            AccessClassOptions = accessClassOptions ?? new DefaultReadAccessClassOptions();
        }
    }

    /// <summary>
    /// Options for loading data keys.
    /// </summary>
    public class LoadOptions : IAccessControlOptions
    {
        /// <summary>
        /// Access class for the operation.
        /// </summary>
        public IAccessClassOptions AccessClassOptions { get; }

        /// <summary>
        /// Options for loading data keys.
        /// </summary>
        public LoadOptions()
        {
            AccessClassOptions = new DefaultReadAccessClassOptions();
        }

        /// <summary>
        /// Options for loading data keys.
        /// </summary>
        /// <param name="accessClassOptions">Access class for the operation.</param>
        public LoadOptions(ReadAccessClassOptions accessClassOptions)
        {
            AccessClassOptions = accessClassOptions ?? new DefaultReadAccessClassOptions();
        }
    }

    /// <summary>
    /// Options for loading data keys.
    /// </summary>
    public class LoadAllOptions : IAccessControlOptions
    {
        /// <summary>
        /// Access class for the operation.
        /// </summary>
        public IAccessClassOptions AccessClassOptions { get; }

        /// <summary>
        /// Options for loading data keys.
        /// </summary>
        public LoadAllOptions()
        {
            AccessClassOptions = new DefaultReadAccessClassOptions();
        }

        /// <summary>
        /// Options for loading data keys.
        /// </summary>
        /// <param name="accessClassOptions">Access class for the operation.</param>
        public LoadAllOptions(ReadAccessClassOptions accessClassOptions)
        {
            AccessClassOptions = accessClassOptions ?? new DefaultReadAccessClassOptions();
        }
    }

    /// <summary>
    /// Options for querying data keys.
    /// </summary>
    public class QueryOptions : IAccessControlOptions
    {
        /// <summary>
        /// Access class for the operation.
        /// </summary>
        public IAccessClassOptions AccessClassOptions { get; }

        /// <summary>
        /// Options for querying data keys.
        /// </summary>
        public QueryOptions()
        {
            AccessClassOptions = new PublicReadAccessClassOptions();
        }
    }

    /// <summary>
    /// Options for writing to specific access classes.
    /// </summary>
    public abstract class WriteAccessClassOptions : IAccessClassOptions
    {
        /// <summary>
        /// Access class for the operation.
        /// </summary>
        public abstract AccessClass AccessClass { get; }

        /// <summary>
        /// Player ID to write to.
        /// </summary>
        public string PlayerId => null;
    }

    /// <summary>
    /// Options for writing default access class data.
    /// </summary>
    public class DefaultWriteAccessClassOptions : WriteAccessClassOptions
    {
        /// <summary>
        /// Access class for the operation.
        /// </summary>
        public override AccessClass AccessClass => AccessClass.Default;
    }

    /// <summary>
    /// Options for writing public access class data.
    /// </summary>
    public class PublicWriteAccessClassOptions : WriteAccessClassOptions
    {
        /// <summary>
        /// Access class for the operation.
        /// </summary>
        public override AccessClass AccessClass => AccessClass.Public;
    }

    /// <summary>
    /// Options for reading data of specific access classes.
    /// </summary>
    public abstract class ReadAccessClassOptions : IAccessClassOptions
    {
        /// <summary>
        /// Access class for the operation.
        /// </summary>
        public abstract AccessClass AccessClass { get; }

        /// <summary>
        /// Player ID to read from.
        /// </summary>
        public abstract string PlayerId { get; }
    }

    /// <summary>
    /// Options for reading default access class data.
    /// </summary>
    public class DefaultReadAccessClassOptions : ReadAccessClassOptions
    {
        /// <summary>
        /// Access class for the operation.
        /// </summary>
        public override AccessClass AccessClass => AccessClass.Default;

        /// <summary>
        /// Player ID to read from.
        /// </summary>
        public override string PlayerId => null;
    }

    /// <summary>
    /// Options for reading protected access class data.
    /// </summary>
    public class ProtectedReadAccessClassOptions : ReadAccessClassOptions
    {
        /// <summary>
        /// Access class for the operation.
        /// </summary>
        public override AccessClass AccessClass => AccessClass.Protected;

        /// <summary>
        /// Player ID to read from.
        /// </summary>
        public override string PlayerId => null;
    }

    /// <summary>
    /// Options for reading public access class data.
    /// </summary>
    public class PublicReadAccessClassOptions : ReadAccessClassOptions
    {
        /// <summary>
        /// Access class for the operation.
        /// </summary>
        public override AccessClass AccessClass => AccessClass.Public;

        /// <summary>
        /// Player ID to read from.
        /// </summary>
        public override string PlayerId { get; }

        /// <summary>
        /// Options for reading public access class data.
        /// </summary>
        public PublicReadAccessClassOptions()
        {
            PlayerId = null;
        }

        /// <summary>
        /// Options for reading public access class data.
        /// </summary>
        /// <param name="playerId">Player ID to read from.</param>
        public PublicReadAccessClassOptions(string playerId)
        {
            PlayerId = playerId;
        }
    }
}
