using System;

namespace Unity.Services.CloudSave.Models
{
    /// <summary>
    /// Class representing a single file.
    /// </summary>
    public class FileItem
    {
        /// <summary>
        /// Creates an instance of FileItem.
        /// </summary>
        internal FileItem(Internal.Models.FileItem item)
        {
            Size = item.Size;
            Created = item.Created.Date;
            Modified = item.Modified.Date;
            WriteLock = item.WriteLock;
            ContentType = item.ContentType;
            Key = item.Key;
        }

        /// <summary>
        /// The size of the file
        /// </summary>
        public long Size { get; }

        /// <summary>
        /// The contentType of the file
        /// </summary>
        public string ContentType { get; }

        /// <summary>
        /// The key against which the file is stored
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// The write lock value for the file, used for enforcing conflict checking when updating an existing data item
        /// </summary>
        public string WriteLock { get; }

        /// <summary>
        /// The datetime when the value was last modified
        /// </summary>
        public DateTime? Modified { get; }

        /// <summary>
        /// The datetime when the value was initially created
        /// </summary>
        public DateTime? Created { get; }
    }
}
