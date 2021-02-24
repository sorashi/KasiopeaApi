using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;

namespace KasiopeaApi.Rest.Model
{
    [DataContract(Name = "APICourse")]
    public class ApiCourse : IEquatable<ApiCourse>, IValidatableObject
    {
        [DataMember(Name = "id", EmitDefaultValue = true)]
        public int Id { get; set; }

        [DataMember(Name = "kind", EmitDefaultValue = true)]
        public CourseKind Kind { get; set; }

        [DataMember(Name = "year", EmitDefaultValue = true)]
        public int Year { get; set; }

        [DataMember(Name = "start_time", EmitDefaultValue = true)]
        public DateTime StartTime { get; set; }

        [DataMember(Name = "end_time", EmitDefaultValue = true)]
        public DateTime EndTime { get; set; }

        [DataMember(Name = "freeze_time", EmitDefaultValue = true)]
        public DateTime FreezeTime { get; set; }

        [DataMember(Name = "unfreeze_time", EmitDefaultValue = true)]
        public DateTime UnfreezeTime { get; set; }

        [DataMember(Name = "tasks", EmitDefaultValue = true)]
        public List<ApiTaskInfo> Tasks { get; set; }

        [DataMember(Name = "state", EmitDefaultValue = true)]
        public CourseState State { get; set; }

        public bool Equals(ApiCourse other) {
            if (other == null) return false;
            return other.Id == Id && other.Kind == Kind && other.Year == Year && other.StartTime == StartTime &&
                   other.EndTime == EndTime && other.FreezeTime == FreezeTime && other.UnfreezeTime == UnfreezeTime &&
                   (other.Tasks == Tasks || other.Tasks != null && Tasks != null && other.Tasks.SequenceEqual(Tasks)) &&
                   other.State == State;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            if(StartTime.Kind != DateTimeKind.Utc) yield return new ValidationResult($"{nameof(StartTime)} is not UTC");
            if (EndTime.Kind != DateTimeKind.Utc) yield return new ValidationResult($"{nameof(EndTime)} is not UTC");
            if (FreezeTime.Kind != DateTimeKind.Utc) yield return new ValidationResult($"{nameof(FreezeTime)} is not UTC");
            if (UnfreezeTime.Kind != DateTimeKind.Utc) yield return new ValidationResult($"{nameof(UnfreezeTime)} is not UTC");
        }

        public override bool Equals(object obj) {
            return Equals(obj as ApiCourse);
        }
    }
}
