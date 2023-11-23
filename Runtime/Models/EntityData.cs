using System.Collections.Generic;
using UnityEngine.Scripting;
using System.Runtime.Serialization;

namespace Unity.Services.CloudSave.Models
{
    /// <summary>
    /// QueryResult model
    /// </summary>
    [Preserve]
    [DataContract(Name = "QueryResult")]
    public class EntityData
    {
        /// <summary>
        /// Creates an instance of QueryResult.
        /// </summary>
        /// <param name="id">id param</param>
        /// <param name="data">The list of data key-value pairs for the entity</param>
        [Preserve]
        public EntityData(string id = default, List<Item> data = default)
        {
            Id = id;
            Data = data;
        }

        /// <summary>
        /// Parameter id of QueryResult
        /// </summary>
        [Preserve]
        [DataMember(Name = "id", EmitDefaultValue = false)]
        public string Id { get; }

        /// <summary>
        /// The list of data key-value pairs for the entity
        /// </summary>
        [Preserve]
        [DataMember(Name = "data", EmitDefaultValue = false)]
        public List<Item> Data { get; }
    }
}
