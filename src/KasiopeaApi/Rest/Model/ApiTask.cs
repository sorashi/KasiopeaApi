using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;

namespace KasiopeaApi.Rest.Model
{
    [DataContract(Name = "APITask")]
    public class ApiTask : IEquatable<ApiTask>, IValidatableObject
    {
        [DataMember(Name = "id", EmitDefaultValue = true)]
        public int Id { get; set; }

        [DataMember(Name="letter", EmitDefaultValue = true)]
        public string Letter { get; set; }

        [DataMember(Name="name", EmitDefaultValue=true)]
        public string Name { get; set; }

        [DataMember(Name="course_id", EmitDefaultValue = true)]
        public int CourseId { get; set; }

        [DataMember(Name = "attempts", EmitDefaultValue = true)]
        public List<Guid> Attempts { get; set; }

        [DataMember(Name = "subtasks", EmitDefaultValue = true)]
        public List<ApiSubtask> Subtasks { get; set; }

        public bool Equals(ApiTask other) {
            if (other == null) return false;

            return other.Id == Id && other.Letter == Letter && other.Name == Name && other.CourseId == CourseId &&
                   (other.Attempts == Attempts || other.Attempts != null && Attempts != null &&
                       other.Attempts.SequenceEqual(Attempts)) &&
                   (other.Subtasks == Subtasks || other.Subtasks != null && Subtasks != null &&
                       other.Subtasks.SequenceEqual(Subtasks));
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            if(Letter.Length != 1)
                yield return new ValidationResult("Letter must be of length 1");

        }
    }
}
