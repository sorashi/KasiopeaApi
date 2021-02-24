using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace KasiopeaApi.Rest.Model
{
    [DataContract(Name = "APIAccessToken")]
    public class ApiAccessToken : IEquatable<ApiAccessToken>, IValidatableObject
    {
        [DataMember(Name = "access_token", EmitDefaultValue = true)]
        public string AccessToken { get; set; }
        [DataMember(Name = "access_token_expiry", EmitDefaultValue = true)]
        public DateTime AccessTokenExpiry { get; set; }

        public bool Equals(ApiAccessToken other) {
            if (other == null) return false;
            return other.AccessToken == AccessToken && other.AccessTokenExpiry == AccessTokenExpiry;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            if (AccessTokenExpiry.Kind != DateTimeKind.Utc)
                yield return new ValidationResult($"{nameof(AccessTokenExpiry)} is not UTC");
        }

        public override bool Equals(object obj) => Equals(obj as ApiAccessToken);

        public override int GetHashCode() => HashCode.Combine(AccessToken, AccessTokenExpiry);
    }
}
