using System.Runtime.Serialization;
using System.Xml.Serialization;
using UnityEngine.Scripting;

namespace Unity.Services.CloudSave.Internal.Models
{
    [Preserve]
    [DataContract(Name = "BasicErrorResponse")]
    [XmlType("Error")]
    public class GCSErrorResponse
    {
        public GCSErrorResponse() {}

        public GCSErrorResponse(string code, string message, string details = null)
        {
            Code = code;
            Message = message;
            Details = details;
        }

        [Preserve]
        [DataMember(Name = "Code", IsRequired = true, EmitDefaultValue = true)]
        [XmlElement("Code")]
        public string Code { get; set; }

        [Preserve]
        [DataMember(Name = "Message", IsRequired = true, EmitDefaultValue = true)]
        [XmlElement("Message")]
        public string Message { get; set; }

        [Preserve]
        [DataMember(Name = "Code", IsRequired = false, EmitDefaultValue = false)]
        [XmlElement("Details")]
        public string Details { get; set; }
    }
}
