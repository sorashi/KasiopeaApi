using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace KasiopeaApi.Rest.Model
{
    [DataContract(Name = "APISchool")]
    public class ApiSchool : IEquatable<ApiSchool>, IValidatableObject
    {
        [DataMember(Name = "name", EmitDefaultValue = true)]
        public string Name { get; set; }
        [DataMember(Name="address", EmitDefaultValue = true)]
        public string Address { get; set; }
        [DataMember(Name="city", EmitDefaultValue = true)]
        public string City { get; set; }

        public bool Equals(ApiSchool other) {
            if (other == null) return false;
            return other.Name == Name && other.Address == Address && other.City == City;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            yield break;
        }
    }
}
