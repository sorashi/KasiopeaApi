using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;

namespace KasiopeaApi.Rest.Model
{
    [DataContract(Name = "APISubtask")]
    public class ApiSubtask : IEquatable<ApiSubtask>, IValidatableObject
    {
        [DataMember(Name = "difficulty", EmitDefaultValue = true)]
        public Difficulty Difficulty { get; set; }

        [DataMember(Name = "received_points", EmitDefaultValue = true)]
        public int ReceivedPoints { get; set; }

        [DataMember(Name = "maximum_points", EmitDefaultValue = true)]
        public int MaximumPoints { get; set; }

        [DataMember(Name = "first_successful_attempt", EmitDefaultValue = false)]
        public Guid FirstSuccessfulAttempt { get; set; }

        public bool Equals(ApiSubtask other) {
            if (other == null) return false;
            return other.Difficulty == Difficulty && other.ReceivedPoints == ReceivedPoints &&
                   other.MaximumPoints == MaximumPoints && other.FirstSuccessfulAttempt == FirstSuccessfulAttempt;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            if(!Enum.GetValues<Difficulty>().Contains(Difficulty))
                yield return new ValidationResult("Invalid difficulty value");
        }
    }
}
