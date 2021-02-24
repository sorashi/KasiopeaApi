using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace KasiopeaApi.Rest.Model
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CourseState
    {
        [EnumMember(Value = "invalid")]
        Invalid = 1,

        [EnumMember(Value = "not_started")]
        NotStarted = 2,

        [EnumMember(Value = "active")]
        Active = 3,

        [EnumMember(Value = "active_frozen")]
        ActiveFrozen = 4,

        [EnumMember(Value = "finished_frozen")]
        FinishedFrozen = 5,

        [EnumMember(Value = "finished")]
        Finished = 6

    }
}
