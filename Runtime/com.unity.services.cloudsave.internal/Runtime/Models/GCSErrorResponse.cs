using System.Runtime.Serialization;
using System.Xml.Serialization;
using UnityEngine.Scripting;

namespace Unity.Services.CloudSave.Internal.Models
{
    /// <summary>
    /// Google Cloud Storage error response.
    /// </summary>
    [Preserve]
    [DataContract(Name = "BasicErrorResponse")]
    [XmlType("Error")]
    public class GCSErrorResponse
    {
        /// <summary>
        /// Google Cloud Storage error response.
        /// </summary>
        public GCSErrorResponse() {}


        /// <summary>
        /// Google Cloud Storage error response.
        /// </summary>
        /// <param name="code">Error code.</param>
        /// <param name="message">Error message.</param>
        /// <param name="details">Error details.</param>
        public GCSErrorResponse(string code, string message, string details = null)
        {
            Code = code;
            Message = message;
            Details = details;
        }

        /// <summary>
        /// Error code.
        /// </summary>
        [Preserve]
        [DataMember(Name = "Code", IsRequired = true, EmitDefaultValue = true)]
        [XmlElement("Code")]
        public string Code { get; set; }

        /// <summary>
        /// Error message.
        /// </summary>
        [Preserve]
        [DataMember(Name = "Message", IsRequired = true, EmitDefaultValue = true)]
        [XmlElement("Message")]
        public string Message { get; set; }

        /// <summary>
        /// Error details.
        /// </summary>
        [Preserve]
        [DataMember(Name = "Details", IsRequired = false, EmitDefaultValue = false)]
        [XmlElement("Details")]
        public string Details { get; set; }
    }
}
