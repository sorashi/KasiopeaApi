using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace KasiopeaApi.Rest.Model
{
    [DataContract(Name = "APIVerificationCode")]
    public class ApiVerificationCode : IEquatable<ApiVerificationCode>, IValidatableObject
    {
        [DataMember(Name="verification_code", EmitDefaultValue = true)]
        public string VerificationCode { get; set; }

        public bool Equals(ApiVerificationCode other) {
            if (other == null) return false;
            return other.VerificationCode == VerificationCode;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            yield break;
        }
    }
}
