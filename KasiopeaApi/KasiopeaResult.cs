using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KasiopeaApi
{
    public class KasiopeaResult
    {
        // TODO: move the logic into KasiopeaCompetition.GetCurrentCompetition
        private const string CurrentCompetitionUrl = "/soutez/";

        public bool Me { get; set; } = false;

        public string Name { get; set; }

        public int Rank { get; set; }

        public Dictionary<char, int> TaskPoints { get; set; } = new Dictionary<char, int>();

        public int TotalMinutes { get; set; }

        public int TotalPoints { get; set; }

        /// <summary>
        /// </summary>
        /// <param name="year"><code>null</code> if current, reference otherwise</param>
        /// <returns></returns>
        public static async Task<IEnumerable<KasiopeaResult>> GetResultList(KasiopeaInterface kInterface,
            KasiopeaCompetition year = null) {
            var doc = new HtmlDocument {OptionFixNestedTags = true};
            // fixes unclosed tr and td tags in the table
            if (year != null) throw new NotImplementedException();
            var code = await kInterface.DownloadStringAsync(CurrentCompetitionUrl + "vysledky.html");
            doc.LoadHtml(code);
            var taskChars = doc.DocumentNode.SelectNodes(@"//table/thead//th[@class='task']")
                .Select(x => x.InnerText[0])
                .ToArray();
            var tbody = doc.DocumentNode.SelectSingleNode(@"//table/tbody");
            var kasiopeaResults = new List<KasiopeaResult>();
            var trNodes = tbody.SelectNodes("tr");
            foreach (var trNode in trNodes) {
                var result = new KasiopeaResult {Me = trNode.GetAttributeValue("class", "") == "me"};
                var currentTask = 0;
                foreach (var tdNode in trNode.SelectNodes("td")) {
                    var text = tdNode.InnerText?.Trim();
                    if (string.IsNullOrWhiteSpace(text)) text = null;
                    switch (tdNode.GetAttributeValue("class", "")) {
                        case "rank":
                            result.Rank = int.Parse(text?.Replace(".", "") ?? "0");
                            break;

                        case "name":
                            result.Name = text;
                            break;

                        case "task":
                            result.TaskPoints.Add(taskChars[currentTask++], int.Parse(text ?? "0"));
                            break;

                        case "total_min":
                            result.TotalMinutes = int.Parse(text ?? "0");
                            break;

                        case "total_points":
                            result.TotalPoints = int.Parse(text ?? "0");
                            break;

                        default:
                            throw new Exception(
                                $"The td class has an unexpected class attribute: '{tdNode.GetAttributeValue("class", "")}'");
                    }
                }
                kasiopeaResults.Add(result);
            }
            return kasiopeaResults;
        }

        public override string ToString() {
            return Rank.ToString().PadLeft(4) + " " + Name.PadRight(25) + " " +
                   TaskPoints.Aggregate("", (a, c) => a + " " + c.Value.ToString().PadRight(3)) + " " +
                   TotalMinutes.ToString().PadRight(5) + " " + TotalPoints.ToString().PadRight(4);
        }
    }
}