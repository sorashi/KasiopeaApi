using System;
using RestSharp;
using RestSharp.Authenticators;

namespace KasiopeaApi.Rest
{
    public class KasiopeaAuthenticator : IAuthenticator
    {
        /// <summary>
        /// Decides whether to use Basic Auth (for testing environments)
        /// </summary>
        public static bool Test { get; set; } = false;

        private const string BasicAuthEnvVarName = "KASIOPEA_TEST_BASICAUTH";
        public string BearerToken { protected get; set; } = null;
        public void Authenticate(IRestClient client, IRestRequest request) {
            if (!Test) {
                request.AddOrUpdateParameter("Authorization", $"Bearer {BearerToken}", ParameterType.HttpHeader);
                return;
            }

            request.AddOrUpdateParameter("X-KASIOPEA-AUTH-TOKEN",
                string.IsNullOrEmpty(BearerToken) ? "" : $"Bearer {BearerToken}", ParameterType.HttpHeader);

            var basicAuth = Environment.GetEnvironmentVariable(BasicAuthEnvVarName);
            if (basicAuth == null)
                throw new InvalidOperationException(
                    $"Missing {BasicAuthEnvVarName} environment variable while {nameof(Test)} is true");
            request.AddOrUpdateParameter("Authorization", $"Basic {basicAuth}", ParameterType.HttpHeader);
        }
    }
}
