using HtmlAgilityPack;
using RestSharp;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace KasiopeaApi
{
    public class KasiopeaTask : KasiopeaResolvableEntity<KasiopeaTask>
    {
        public enum InputVersion
        {
            Easy = 1,
            Hard = 2
        }

        public enum OutputCheckResult
        {
            Success,
            Fail,
            Timeout,
            MissingFile,
            Unknown
        }

        public KasiopeaTask(char id, string name, string url, KasiopeaStage stage = null) : base(url) {
            Id = id;
            Name = name;
            Stage = stage;
            if (!string.IsNullOrWhiteSpace(Url) && !Url.EndsWith("/")) Url += "/";
        }

        public string FullDescriptionHtml { get; private set; }

        public string FullDescriptionInnerText { get; private set; }

        /// <summary>
        ///     The letter of the task, which it is identified with
        /// </summary>
        public char Id { get; }

        public string Name { get; }

        public KasiopeaStage Stage { get; }

        private string CachedHtml { get; set; }
        private StringWriter OutputWriter { get; set; }

        public static async Task<KasiopeaTask> FromUrl(string relativeUrl, KasiopeaInterface kInterface) {
            if (string.IsNullOrWhiteSpace(relativeUrl)) return null;
            if (!relativeUrl.EndsWith("/")) relativeUrl += "/";
            var doc = new HtmlDocument {OptionFixNestedTags = true};
            doc.LoadHtml(await kInterface.DownloadStringAsync(relativeUrl));
            var nodes = doc.DocumentNode.SelectNodes("//h1");
            var regex = new Regex(@"^(?<id>[A-Z]): (?<name>.*)$");
            foreach (var innerText in nodes.Select(x => x.InnerText.Trim())) {
                var match = regex.Match(innerText);
                if (match.Success)
                    return new KasiopeaTask(match.Groups["id"].Value[0], match.Groups["name"].Value, relativeUrl);
            }
            throw new FormatException("Failed to parse task information from h1");
        }

        public void ClearStreamWriter() {
            OutputWriter?.Dispose();
            OutputWriter = new StringWriter();
        }

        public string GetConstraints() {
            throw new NotImplementedException();
        }

        public async Task<string> GetInputAsync(InputVersion version, KasiopeaInterface kInterface) {
            // WARNING: there has to be a slash (/) at the end of the url in order for the server to respond appropriately; query params appended with ?; example: /soutez/A/?something=1234
            // this is already ensured in the constructor
            var request = new RestRequest(Url, Method.GET);
            // generate the task
            request.AddQueryParameter("do", "gen");
            request.AddQueryParameter("subtask", ((int) version).ToString());
            await kInterface.ExecuteRequest(request);

            // download the input
            request = new RestRequest(Url, Method.GET);
            request.AddQueryParameter("do", "get");
            request.AddQueryParameter("subtask", ((int) version).ToString());

            var resp = await kInterface.ExecuteRequest(request);
            return resp.Content;
        }

        public string GetInputFormatDescription() {
            throw new NotImplementedException();
        }

        public async Task<StringReader> GetInputReaderAsync(InputVersion version, KasiopeaInterface kInterface) {
            return new StringReader(await GetInputAsync(version, kInterface));
        }

        public string GetIoExplanation() {
            throw new NotImplementedException();
        }

        public string GetOutputFormatDescription() {
            throw new NotImplementedException();
        }

        public StringWriter GetOutputWriter() {
            return OutputWriter ?? (OutputWriter = new StringWriter());
        }

        public string GetSampleInput() {
            throw new NotImplementedException();
        }

        public string GetSampleOutput() {
            throw new NotImplementedException();
        }

        public string GetTaskDescription() {
            throw new NotImplementedException();
        }

        public async Task<OutputCheckResult> PostOutputAsync(InputVersion version, KasiopeaInterface kInterface) {
            var fn = Path.GetTempFileName();
            await OutputWriter.FlushAsync();
            File.WriteAllText(fn, OutputWriter.ToString());
            ClearStreamWriter();
            return await PostOutputAsync(fn, version, kInterface);
        }

        public async Task<OutputCheckResult> PostOutputAsync(string outputPath, InputVersion version,
            KasiopeaInterface kInterface, Encoding e = null, string sourcePath = null) {
            var request = new RestRequest(Url, Method.POST);
            request.AddQueryParameter("do", "send");
            request.AddQueryParameter("subtask", ((int) version).ToString());
            request.AddFile("f", outputPath);
            if (!string.IsNullOrWhiteSpace(sourcePath)) request.AddFile("s", sourcePath);
            var res = await kInterface.ExecuteRequest(request);
            var doc = new HtmlDocument {OptionFixNestedTags = true};
            doc.LoadHtml(res.Content);
            var messages = doc.DocumentNode.SelectNodes("//p[@class='error']")
                ?.Select(x => x.InnerText)
                .Concat(
                    doc.DocumentNode.SelectNodes("//p[@class='okay']")?.Select(x => x.InnerText) ?? Enumerable.Empty<string>()
                );
            if(messages == null || !messages.Any()) throw new FormatException("Response doesn't contain any message stating the result");
            foreach (var message in messages)
                switch (message.Trim()) {
                    case "Výborně, odeslal jsi správné řešení!":
                        return OutputCheckResult.Success;

                    case "Bohužel, odevzdal jsi špatné řešení.":
                        return OutputCheckResult.Fail;

                    case "Nenahrál jsi soubor s výsledkem":
                    case "Nenahrál jsi zdrojový soubor":
                        return OutputCheckResult.MissingFile;

                    case "Platnost tohoto testu už vypršela. Vygeneruj si nový vstup.":
                        return OutputCheckResult.Timeout;

                    case "Soutež skončila. Získané body již nejdou do výsledků.":
                    case "Nejsi finalista, takže se neobjevíš ve výsledcích.":
                        continue;
                    default:
                        continue;
                }
            return OutputCheckResult.Unknown;
        }

        public override async Task<KasiopeaTask> Resolve(KasiopeaInterface kInterface) {
            if (string.IsNullOrWhiteSpace(Url)) throw new InvalidOperationException("No Task Url specified");
            var res = kInterface.DownloadStringAsync(Url);
            var doc = new HtmlDocument {OptionFixNestedTags = true};
            doc.LoadHtml(await res);
            CachedHtml = await res;
            var div = doc.DocumentNode.SelectSingleNode("//div[@class='col-md-9']");
            // remove all information boxes and everything that doesn't belong into the description
            div.SelectNodes("p[@class='error']").ForEach(x => x?.Remove());
            div.SelectNodes("table")
                ?.Where(x => x.GetAttributeValue("class", "").StartsWith("taskbox"))
                .ForEach(x => x?.Remove());
            div.SelectNodes("div[@class='sub-navbar']")?.ForEach(x => x.Remove());
            FullDescriptionHtml = div.InnerHtml;
            FullDescriptionInnerText = HttpUtility.HtmlDecode(div.InnerText);
            // sets the Resolved property to true
            await base.Resolve(kInterface);
            return this;
        }
    }
}