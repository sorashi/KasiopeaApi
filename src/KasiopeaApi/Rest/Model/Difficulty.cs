using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace KasiopeaApi.Rest.Model
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Difficulty
    {
        [EnumMember(Value = "easy")]
        Easy = 1,

        [EnumMember(Value = "hard")]
        Hard = 2
    }
}
