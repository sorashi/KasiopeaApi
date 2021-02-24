using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace KasiopeaApi.Rest.Model
{
    [DataContract(Name = "APITaskInfo")]
    public class ApiTaskInfo : IEquatable<ApiTaskInfo>, IValidatableObject
    {
        [DataMember(Name="id", EmitDefaultValue = true)]
        public int Id { get; set; }

        [DataMember(Name="letter", EmitDefaultValue = true)]
        public string Letter { get; set; }

        [DataMember(Name = "name", EmitDefaultValue = true)]
        public string Name { get; set; }

        public bool Equals(ApiTaskInfo other) {
            if (other == null) return false;
            return other.Id == Id && other.Letter == Letter && other.Name == Name;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            if (Letter.Length != 1) yield return new ValidationResult($"{nameof(Letter)} is not a letter");
        }
    }
}
