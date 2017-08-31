using HtmlAgilityPack;
using RestSharp;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace KasiopeaApi
{
    public class KasiopeaInterface : KasiopeaEntity
    {
        private const string LoginFormUrl = "/auth/login.cgi?done=1";
        private readonly RestClient _restClient;

        private bool _loggedIn;

        public KasiopeaInterface(bool disableCache = false) : base(BaseUrl) {
            _restClient = new RestClient(BaseUrl) {
                CookieContainer = new CookieContainer()
            };
        }

        public bool LoggedIn {
            get => _loggedIn && _restClient.CookieContainer.Count > 0;
            private set => _loggedIn = value;
        }

        public async Task<string> DownloadStringAsync(string resource) {
            var request = new RestRequest(resource, Method.GET);
            return (await _restClient.ExecuteTaskAsync(request)).Content;
        }

        public async Task<IRestResponse> ExecuteRequest(IRestRequest request) {
            return await _restClient.ExecuteTaskAsync(request);
        }

        /// <summary>
        ///     Gets the session cookies from the server and stores it
        /// </summary>
        /// <returns>True if the login is successful</returns>
        public async Task<bool> Login(string email, string password) {
            var data = new NameValueCollection {
                {"email", email},
                {"passwd", password},
                {"redirect", ""},
                {"submit", "Přihlásit"}
            };
            var response = await UploadValuesAsync(LoginFormUrl, data);
            var doc = new HtmlDocument();
            doc.LoadHtml(response);
            var buts = doc.DocumentNode.Descendants("button");
            var result = buts.Any(x => x.InnerText == "Přihlášen") && buts.Any(x => x.InnerText == "Odhlásit");
            LoggedIn = result;
            return result;
        }

        public async Task<string> UploadValuesAsync(string resource, NameValueCollection data) {
            var request = new RestRequest(resource, Method.POST);
            foreach (var key in data.AllKeys) request.AddParameter(key, data[key]);
            return (await _restClient.ExecuteTaskAsync(request)).Content;
        }
    }
}