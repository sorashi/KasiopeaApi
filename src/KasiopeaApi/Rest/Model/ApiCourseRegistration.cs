using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;

namespace KasiopeaApi.Rest.Model
{
    [DataContract(Name = "APISchoolRegistration")]
    public class ApiCourseRegistration : IEquatable<ApiCourseRegistration>, IValidatableObject
    {
        [DataMember(Name = "school", EmitDefaultValue = false)]
        public ApiSchool School { get; set; }
        [DataMember(Name = "maturita", EmitDefaultValue = false)]
        public int Maturita { get; set; }

        [DataMember(Name = "teacher", EmitDefaultValue = false)]
        public string Teacher { get; set; }
        [DataMember(Name = "contestant_role", EmitDefaultValue = true)]
        public ContestantRole ContestantRole { get; set; }

        public bool Equals(ApiCourseRegistration other) {
            if (other == null) return false;
            return other.School.Equals(School) && other.Maturita == Maturita && other.Teacher == Teacher &&
                   other.ContestantRole == ContestantRole;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) =>
            Enumerable.Empty<ValidationResult>();
    }
}
