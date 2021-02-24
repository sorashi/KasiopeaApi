using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using KasiopeaApi.Rest.Model;
using RestSharp;
using Newtonsoft.Json;

namespace KasiopeaApi.Rest
{
    public class Client
    {
        private const int ApiVersion = 1;
        private Uri BaseUrl { get; }
        private readonly RestClient client;

        private KasiopeaAuthenticator Authenticator {
            get {
                client.Authenticator ??= new KasiopeaAuthenticator();
                return client.Authenticator as KasiopeaAuthenticator;
            }
            set => client.Authenticator = value;
        }

        public Client(Uri baseUrl, ApiCredentials credentials) {
            BaseUrl = baseUrl;
            this.credentials = credentials;
            client = new RestClient(baseUrl) {ThrowOnAnyError = true, Authenticator = new KasiopeaAuthenticator()};
        }

        public async Task<ApiTask> TaskGet(int taskId) {
            var request = CreateRequest("/tasks/{taskId}").AddUrlSegment(nameof(taskId), taskId);
            await RefreshToken();
            return await client.GetAsync<ApiTask>(request);
        }

        public async Task<string> TaskGetAssignmentHtml(int taskId) {
            var request = CreateRequest("/tasks/{taskId}/assignment").AddUrlSegment(nameof(taskId), taskId);
            await RefreshToken();
            return await client.GetAsync<string>(request);
        }

        public async Task<string> TaskGetSolutionHtml(int taskId) {
            var request = CreateRequest("/tasks/{taskId}/solution").AddUrlSegment(nameof(taskId), taskId);
            await RefreshToken();
            return await client.GetAsync<string>(request);
        }

        public async Task<List<Guid>> TaskGetAttempts(int taskId) {
            var request = CreateRequest("/tasks/{taskId}/attempts").AddUrlSegment(nameof(taskId), taskId);
            await RefreshToken();
            return await client.GetAsync<List<Guid>>(request);
        }

        public async Task<ApiAttempt> TaskPostNewAttempt(int taskId, Difficulty difficulty) {
            var request = CreateRequest("/tasks/{taskId}/{difficulty}").AddUrlSegment(nameof(taskId), taskId)
                .AddUrlSegment(nameof(difficulty), GetEnumMemberValue(typeof(Difficulty), difficulty));
            await RefreshToken();
            var response = await client.ExecutePostAsync(request);
            if(response.StatusCode != HttpStatusCode.OK) throw new Exception(response.Content);
            return client.Deserialize<ApiAttempt>(response).Data;
        }

        public async Task<ApiAttempt> AttemptGet(Guid attemptId) {
            var request = CreateRequest("/attempts/{attemptId}").AddUrlSegment(nameof(attemptId), attemptId);
            await RefreshToken();
            return await client.GetAsync<ApiAttempt>(request);
        }

        /// <summary>
        /// Downloads the attempt input to a temporary file on disk
        /// </summary>
        /// <returns>Temporary file path</returns>
        public async Task<string> AttemptGetInput(Guid attemptId) {
            var request = CreateRequest("/attempts/{attemptId}/input").AddUrlSegment(nameof(attemptId), attemptId);
            request.AddHeader("Accept", "text/plain");
            await RefreshToken();

            var tempFile = Path.GetTempFileName();

            request.ResponseWriter = responseStream => {
                using var fileWriter = File.OpenWrite(tempFile);
                using (responseStream) responseStream.CopyTo(fileWriter);
                fileWriter.Flush();
            };
            request.OnBeforeRequest += http => {
                Debug.WriteLine(http.Headers.Aggregate("", (a, c) => a + $"{c.Name}: {c.Value}\n"));
            };
            await client.ExecuteGetAsync(request);
            return tempFile;
        }

        public async Task<ApiAttempt> AttemptPostSubmit(Guid attemptId, string filePath) {
            var request = CreateRequest("/attempts/{attemptId}/submit").AddUrlSegment(nameof(attemptId), attemptId);
            await RefreshToken();
            request.AddFile("output", filePath);
            request.AddHeader("Content-Type", "multipart/form-data");
            return await client.PostAsync<ApiAttempt>(request);
        }

        public async Task AttemptPostCancel(Guid attemptId) {
            var request = CreateRequest("/attempts/{attemptId}/cancel").AddUrlSegment(nameof(attemptId), attemptId);
            await RefreshToken();
            await client.ExecutePostAsync(request);
        }

        public async Task<List<ApiCourse>> CoursesGet(int year) {
            var request = CreateRequest("/courses").AddParameter(nameof(year), year);
            await RefreshToken();
            return await client.GetAsync<List<ApiCourse>>(request);
        }

        public async Task<ApiCourse> CoursesGetSingle(int courseId) {
            var request = CreateRequest("/courses/{courseId}").AddUrlSegment(nameof(courseId), courseId);
            await RefreshToken();
            return await client.GetAsync<ApiCourse>(request);
        }

        public async Task<int> CoursesPostRegister(int courseId, ApiCourseRegistration body) {
            var request = CreateRequest("/courses/{courseId}/register").AddJsonBody(body);
            await RefreshToken();
            // returns int for some reason, TODO: find out its semantic
            return await client.PostAsync<int>(request);
        }
        /// <summary>
        /// Get the scoreboard of a single available course.
        /// </summary>
        public async Task<ApiScoreboard> CoursesGetScoreboard(int courseId, bool all = false) { // TODO: find out semantic of the all parameter
            var request = CreateRequest("/courses/{courseId}/scoreboard").AddUrlSegment(nameof(courseId), courseId)
                .AddQueryParameter(nameof(all), all ? "true" : "false");
            await RefreshToken();
            return await client.GetAsync<ApiScoreboard>(request);
        }
        /// <summary>
        /// Get the scoreboard of a single available course for the current user.
        /// </summary>
        public async Task<ApiScoreboard> CoursesGetScoreboardSingle(int courseId) {
            var request = CreateRequest("/courses/{courseId}/scoreboard-single")
                .AddUrlSegment(nameof(courseId), courseId);
            await RefreshToken();
            return await client.GetAsync<ApiScoreboard>(request);
        }
        public async Task<ApiAccessToken> AuthenticatePost(ApiCredentials body) {
            var request = CreateRequest("/auth");
            request.AddJsonBody(body);
            var response = await client.ExecutePostAsync(request);
            return client.Deserialize<ApiAccessToken>(response).Data;
        }

        public async Task<int> AuthPostRegister(ApiRegistration body) {
            var request = CreateRequest("/auth/register").AddJsonBody(body);
            return await client.PostAsync<int>(request);
        }

        public async Task AuthPostVerify(ApiVerificationCode body) {
            var request = CreateRequest("/auth/verify").AddJsonBody(body);
            await client.ExecutePostAsync(request);
        }

        public async Task<ApiAccessToken> AuthPostRefresh() {
            var request = CreateRequest("/auth/refresh");
            if(token == null || token.AccessTokenExpiry < DateTime.UtcNow) throw new InvalidOperationException("Token is null or already expired");
            var response = await client.ExecutePostAsync(request);
            return client.Deserialize<ApiAccessToken>(response).Data;
        }

        public async Task AuthPostLogout() {
            var request = CreateRequest("/auth/logout");
            await RefreshToken();
            await client.ExecutePostAsync(request);
        }

        /// <summary>
        /// Creates a new request and sets the JSON parser to <see cref="NewtonsoftSerializer"/>
        /// </summary>
        private static IRestRequest CreateRequest(string resourcePath) {
            var request = new RestRequest(resourcePath, DataFormat.Json) {JsonSerializer = new NewtonsoftSerializer()};
            return request;
        }

        private ApiAccessToken token;
        private readonly ApiCredentials credentials;
        
        /// <summary>
        /// Populates the <paramref name="request"/> with a cached token, if it's valid, otherwise requests a new token or refreshes it
        /// </summary>
        /// <returns>Modified  <paramref name="request"/></returns>
        private async Task RefreshToken() {
            if (token == null || token.AccessTokenExpiry < DateTime.UtcNow) {
                if(credentials == null) throw new InvalidOperationException("Cannot authorize, credentials are null");
                token = await AuthenticatePost(credentials);
                Authenticator.BearerToken = token.AccessToken;
            } else if ((token.AccessTokenExpiry - DateTime.UtcNow).TotalMinutes < 5) {
                // < 5 is an arbitrary decision, TODO find better limit
                token = await AuthPostRefresh();
                Authenticator.BearerToken = token.AccessToken;
            }
        }

        private static string GetEnumMemberValue(Type enumType, object enumValue) {
            return enumType.GetMember(enumValue?.ToString() ?? throw new ArgumentNullException(nameof(enumValue)))
                .Single()
                .GetCustomAttributes(false)
                .OfType<EnumMemberAttribute>()
                .FirstOrDefault()?.Value;
        }
    }
}
