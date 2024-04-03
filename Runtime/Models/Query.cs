using System.Collections.Generic;
using UnityEngine.Scripting;
using System.Runtime.Serialization;

namespace Unity.Services.CloudSave.Models
{
    /// <summary>
    /// The request body for querying an index
    /// </summary>
    [Preserve]
    [DataContract(Name = "Query")]
    public class Query
    {
        /// <summary>
        /// The request body for querying an index
        /// </summary>
        /// <param name="fields">fields param</param>
        /// <param name="returnKeys">The keys to return in the response. This can include keys not on the index. If not specified or empty, the data on the results will be empty for any returned entities.</param>
        /// <param name="offset">The number of results to skip. Defaults to 0.</param>
        /// <param name="limit">The maximum number of results to return. Defaults to 10. Specifying 0 will return the default number of results.</param>
        [Preserve]
        public Query(List<FieldFilter> fields, HashSet<string> returnKeys = default, int offset = default,
                     int limit = default)
        {
            Fields = fields;
            ReturnKeys = returnKeys;
            Offset = offset;
            Limit = limit;
        }

        /// <summary>
        /// The request body for querying an index
        /// </summary>
        /// <param name="fields">fields param</param>
        /// <param name="returnKeys">The keys to return in the response. This can include keys not on the index. If not specified or empty, the data on the results will be empty for any returned entities.</param>
        /// <param name="offset">The number of results to skip. Defaults to 0.</param>
        /// <param name="limit">The maximum number of results to return. Defaults to 10. Specifying 0 will return the default number of results.</param>
        /// <param name="sampleSize">If set, the given number of random items will be chosen from the total query results and returned as a sample. Defaults to null.</param>
        [Preserve]
        public Query(List<FieldFilter> fields, HashSet<string> returnKeys, int offset,
                     int limit, int? sampleSize)
        {
            Fields = fields;
            ReturnKeys = returnKeys;
            Offset = offset;
            Limit = limit;
            SampleSize = sampleSize;
        }
        
        /// <summary>
        /// Parameter fields of Query
        /// </summary>
        [Preserve]
        [DataMember(Name = "fields", IsRequired = true, EmitDefaultValue = true)]
        public List<FieldFilter> Fields { get; }

        /// <summary>
        /// The keys to return in the response. This can include keys not on the index. If not specified or empty, the data on the results will be empty for any returned entities.
        /// </summary>
        [Preserve]
        [DataMember(Name = "returnKeys", EmitDefaultValue = false)]
        public HashSet<string> ReturnKeys { get; }

        /// <summary>
        /// The number of results to skip. Defaults to 0.
        /// </summary>
        [Preserve]
        [DataMember(Name = "offset", EmitDefaultValue = false)]
        public int Offset { get; }

        /// <summary>
        /// The maximum number of results to return. Defaults to 10. Specifying 0 will return the default number of results.
        /// </summary>
        [Preserve]
        [DataMember(Name = "limit", EmitDefaultValue = false)]
        public int Limit { get; }

        /// <summary>
        /// If set, the given number of random items will be chosen from the total query results and returned as a sample. Defaults to null.
        /// </summary>
        [Preserve]
        [DataMember(Name = "sampleSize", EmitDefaultValue = false)]
        public int? SampleSize { get; }
    }
}
