using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;

namespace KasiopeaApi.Rest.Model
{
    [DataContract(Name = "APIScoreboard")]
    public class ApiScoreboard : IEquatable<ApiScoreboard>, IValidatableObject
    {
        [DataMember(Name = "task_letters", EmitDefaultValue = true)]
        public List<string> TaskLetters { get; set; }

        [DataMember(Name="contestants", EmitDefaultValue = true)]
        public List<ApiScoreboardRow> Contestants { get; set; }

        public bool Equals(ApiScoreboard other) {
            if (other == null) return false;
            return (other.TaskLetters == TaskLetters || other.TaskLetters != null && TaskLetters != null &&
                       other.TaskLetters.SequenceEqual(TaskLetters)) &&
                   (other.Contestants == Contestants || other.Contestants != null && Contestants != null &&
                       other.Contestants.SequenceEqual(Contestants));
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            if (TaskLetters != null)
                for (var i = 0; i < TaskLetters.Count; i++)
                    if (TaskLetters[i].Length != 0)
                        yield return new ValidationResult($"{nameof(TaskLetters)}[{i}] is not a letter");

            if (Contestants == null) yield break;
            foreach (ValidationResult validationResult in Contestants.SelectMany(x =>
                x.Validate(new ValidationContext(this))))
                yield return validationResult;
        }
    }
}
