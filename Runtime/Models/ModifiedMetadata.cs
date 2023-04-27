using System;
using System.Runtime.Serialization;
using UnityEngine.Scripting;

namespace Unity.Services.CloudSave.Models
{
    /// <summary>
    /// Timestamp for when the object was modified.
    /// </summary>
    [Preserve]
    [DataContract(Name = "ModifiedMetadata")]
    public class ModifiedMetadata
    {
        /// <summary>
        /// Timestamp for when the object was modified.
        /// </summary>
        /// <param name="date">Date time in ISO 8601 format. Null if there is no associated value.</param>
        [Preserve]
        public ModifiedMetadata(DateTime? date)
        {
            Date = date;
        }

        internal ModifiedMetadata(Internal.Models.ModifiedMetadata metadata)
        {
            Date = metadata.Date;
        }

        /// <summary>
        /// Date time in ISO 8601 format. Null if there is no associated value.
        /// </summary>
        [Preserve]
        [DataMember(Name = "date", IsRequired = true, EmitDefaultValue = true)]
        public DateTime? Date{ get; }
    }
}
