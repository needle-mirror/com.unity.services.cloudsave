using System.Runtime.Serialization;
using UnityEngine.Scripting;

namespace Unity.Services.CloudSave.Models
{
    /// <summary>
    /// FileItem model
    /// </summary>
    [Preserve]
    [DataContract(Name = "file-item")]
    public class FileItem
    {
        /// <summary>
        /// Creates an instance of FileItem.
        /// </summary>
        /// <param name="size">size param</param>
        /// <param name="created">created param</param>
        /// <param name="modified">modified param</param>
        /// <param name="writeLock">writeLock param</param>
        /// <param name="contentType">contentType param</param>
        /// <param name="key">key param</param>
        [Preserve]
        public FileItem(long size, ModifiedMetadata created, ModifiedMetadata modified, string writeLock, string contentType, string key = default)
        {
            Size = size;
            Created = created;
            Modified = modified;
            WriteLock = writeLock;
            ContentType = contentType;
            Key = key;
        }

        internal FileItem(Internal.Models.FileItem item)
        {
            Size = item.Size;
            Created = new ModifiedMetadata(item.Created);
            Modified = new ModifiedMetadata(item.Modified);
            WriteLock = item.WriteLock;
            ContentType = item.ContentType;
            Key = item.Key;
        }

        /// <summary>
        /// Parameter size of FileItem
        /// </summary>
        [Preserve]
        [DataMember(Name = "size", IsRequired = true, EmitDefaultValue = true)]
        public long Size { get; }

        /// <summary>
        /// Parameter created of FileItem
        /// </summary>
        [Preserve]
        [DataMember(Name = "created", IsRequired = true, EmitDefaultValue = true)]
        public ModifiedMetadata Created { get; }

        /// <summary>
        /// Parameter modified of FileItem
        /// </summary>
        [Preserve]
        [DataMember(Name = "modified", IsRequired = true, EmitDefaultValue = true)]
        public ModifiedMetadata Modified { get; }

        /// <summary>
        /// Parameter writeLock of FileItem
        /// </summary>
        [Preserve]
        [DataMember(Name = "writeLock", IsRequired = true, EmitDefaultValue = true)]
        public string WriteLock { get; }

        /// <summary>
        /// Parameter contentType of FileItem
        /// </summary>
        [Preserve]
        [DataMember(Name = "contentType", IsRequired = true, EmitDefaultValue = true)]
        public string ContentType { get; }

        /// <summary>
        /// Parameter key of FileItem
        /// </summary>
        [Preserve]
        [DataMember(Name = "key", EmitDefaultValue = false)]
        public string Key { get; }
    }
}
