using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;

namespace KasiopeaApi.Rest.Model
{
    [DataContract(Name = "APIScoreboardRow")]
    public class ApiScoreboardRow : IEquatable<ApiScoreboardRow>, IValidatableObject
    {
        [DataMember(Name = "name", EmitDefaultValue = true)]
        public string Name { get; set; }

        [DataMember(Name = "map_points", EmitDefaultValue = true)]
        public Dictionary<string, int> MapPoints { get; set; }

        [DataMember(Name = "minutes", EmitDefaultValue = false)]
        public int Minutes { get; set; }

        [DataMember(Name = "total_points", EmitDefaultValue = true)]
        public int TotalPoints { get; set; }

        public bool Equals(ApiScoreboardRow other) {
            if (other == null) return false;
            return other.Name == Name &&
                   (other.MapPoints == MapPoints || other.MapPoints != null &&
                       MapPoints != null && other.MapPoints.Count == MapPoints.Count &&
                       !other.MapPoints.Except(MapPoints).Any())
                   && other.Minutes == Minutes && other.TotalPoints == TotalPoints;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            yield break;
        }
    }
}
