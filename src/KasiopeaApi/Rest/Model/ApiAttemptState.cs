using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace KasiopeaApi.Rest.Model
{
    [JsonConverter(typeof(StringEnumConverter)), DataContract(Name = "APIAttemptState")]
    public enum ApiAttemptState
    {
        [EnumMember(Value = "success")]
        Success = 1,

        [EnumMember(Value = "failure")]
        Failure = 2,

        [EnumMember(Value = "pending")]
        Pending = 3,

        [EnumMember(Value = "timeout")]
        Timeout = 4,

        [EnumMember(Value = "internal_error")]
        InternalError = 5,

        [EnumMember(Value = "server_cancelled")]
        ServerCancelled = 6,

        [EnumMember(Value = "user_cancelled")]
        UserCancelled = 7
    }
}
