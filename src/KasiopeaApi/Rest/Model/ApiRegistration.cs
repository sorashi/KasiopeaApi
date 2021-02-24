using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;

namespace KasiopeaApi.Rest.Model
{
    [DataContract(Name = "APIRegistration")]
    public class ApiRegistration : IEquatable<ApiRegistration>, IValidatableObject
    {
        [DataMember(Name = "email", EmitDefaultValue = true)]
        public string Email { get; set; }
        [DataMember(Name = "password", EmitDefaultValue = true)]
        public string Password { get; set; }
        [DataMember(Name = "first_name", EmitDefaultValue = true)]
        public string FirstName { get; set; }
        [DataMember(Name = "last_name", EmitDefaultValue = true)]
        public string LastName { get; set; }
        [DataMember(Name = "knows_from", EmitDefaultValue = true)]
        public string KnowsFrom { get; set; }
        [DataMember(Name = "other_gigs", EmitDefaultValue = true)]
        public string OtherGigs { get; set; }
        [DataMember(Name = "mff_spam", EmitDefaultValue = true)]
        public bool MffSpam { get; set; }
        public bool Equals(ApiRegistration other) {
            if (other == null) return false;
            return other.Email == Email && other.Password == Password && other.FirstName == FirstName &&
                other.LastName == LastName && other.KnowsFrom == KnowsFrom && other.OtherGigs == OtherGigs &&
                other.MffSpam == MffSpam;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            return Enumerable.Empty<ValidationResult>();
        }
    }
}
