using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace KasiopeaApi.Rest.Model
{

    [JsonConverter(typeof(StringEnumConverter))]

    public enum ContestantRole
    {
        /// <summary>
        /// Enum Student for value: student
        /// </summary>
        [EnumMember(Value = "student")]
        Student = 1,

        [EnumMember(Value = "teacher")]
        Teacher = 2,

        [EnumMember(Value = "tester")]
        Tester = 3,

        [EnumMember(Value = "other")]
        Other = 4

    }
}
