using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;

namespace KasiopeaApi.Rest.Model
{
    [DataContract(Name = "APICredentials")]
    public class ApiCredentials : IEquatable<ApiCredentials>, IValidatableObject

    {
        [DataMember(Name = "email", EmitDefaultValue = true)]
        public string Email { get; set; }

        [DataMember(Name = "password", EmitDefaultValue = true)]
        public string Password { get; set; }

        public bool Equals(ApiCredentials other) {
            if (other == null) return false;
            return other.Email == Email && other.Password == Password;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            return Enumerable.Empty<ValidationResult>();
        }
    }
}
