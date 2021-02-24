using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp.Serializers;

namespace KasiopeaApi.Rest
{
    public class NewtonsoftSerializer : ISerializer
	{
		private readonly JsonSerializer serializer;

		/// <summary>
		/// Default serializer
		/// </summary>
		public NewtonsoftSerializer() {
			ContentType = "application/json";
			serializer = new JsonSerializer {
				MissingMemberHandling = MissingMemberHandling.Ignore,
				NullValueHandling = NullValueHandling.Include,
				DefaultValueHandling = DefaultValueHandling.Include
			};
		}

		/// <summary>
		/// Default serializer with overload for allowing custom Json.NET settings
		/// </summary>
		public NewtonsoftSerializer(JsonSerializer serializer) {
			ContentType = "application/json";
			this.serializer = serializer;
		}

		/// <summary>
		/// Serialize the object as JSON
		/// </summary>
		/// <param name="obj">Object to serialize</param>
		/// <returns>JSON as String</returns>
		public string Serialize(object obj) {
            using var stringWriter = new StringWriter();
            using var jsonTextWriter = new JsonTextWriter(stringWriter) {Formatting = Formatting.None, QuoteChar = '"'};

            serializer.Serialize(jsonTextWriter, obj);

            var result = stringWriter.ToString();
            return result;
        }

		/// <summary>
		/// Content type for serialized content
		/// </summary>
		public string ContentType { get; set; }
	}
}
