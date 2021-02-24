using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace KasiopeaApi.Rest.Model
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CourseKind
    {
        [EnumMember(Value = "home")]
        Home = 1,

        [EnumMember(Value = "final")]
        Final = 2,

        [EnumMember(Value = "public_testing")]
        PublicTesting = 3
    }
}
