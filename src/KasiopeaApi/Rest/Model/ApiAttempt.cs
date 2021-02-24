using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;

namespace KasiopeaApi.Rest.Model
{
    [DataContract(Name="APIAttempt")]
    public class ApiAttempt : IEquatable<ApiAttempt>, IValidatableObject
    {
        [DataMember(Name = "id", EmitDefaultValue = true)]
        public Guid Id { get; set;  }

        [DataMember(Name="task_id", EmitDefaultValue = true)]
        public int TaskId { get; set;  }

        [DataMember(Name="contestant_id", EmitDefaultValue = true)]
        public int ContestantId { get; set; }

        [DataMember(Name = "difficulty")]
        public Difficulty Difficulty { get; set; }

        [DataMember(Name = "input_valid_until", EmitDefaultValue = true)]
        public DateTime InputValidUntil { get; set; }

        [DataMember(Name = "submitted_on", EmitDefaultValue = false)]
        public DateTime SubmittedOn { get; set; }

        [DataMember(Name = "state", EmitDefaultValue = true)]
        public ApiAttemptState State { get; set; }

        [DataMember(Name = "received_points", EmitDefaultValue = true)]
        public int ReceivedPoints { get; set; }

        public bool Equals(ApiAttempt other) {
            if (other == null) return false;
            return other.Id == Id && other.TaskId == TaskId && other.ContestantId == ContestantId &&
                   other.Difficulty == Difficulty && other.InputValidUntil == InputValidUntil &&
                   other.SubmittedOn == SubmittedOn && other.State == State && other.ReceivedPoints == ReceivedPoints;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            if (InputValidUntil.Kind != DateTimeKind.Utc)
                yield return new ValidationResult("Date time is not UTC", new[] {nameof(InputValidUntil)});
            if (SubmittedOn.Kind != DateTimeKind.Utc)
                yield return new ValidationResult("Date time is not UTC", new[] { nameof(SubmittedOn) });
            if (!Enum.GetValues<Difficulty>().Contains(Difficulty))
                yield return new ValidationResult("Invalid difficulty value");
            if (!Enum.GetValues<ApiAttemptState>().Contains(State))
                yield return new ValidationResult("Invalid state value");
        }

        public override bool Equals(object obj) => Equals(obj as ApiAttempt);

        public override int GetHashCode() => HashCode.Combine(Id, TaskId, ContestantId, (int) Difficulty, InputValidUntil, SubmittedOn, (int) State, ReceivedPoints);
    }
}
