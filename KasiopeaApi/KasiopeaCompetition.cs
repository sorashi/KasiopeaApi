using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KasiopeaApi
{
    /// <summary>
    ///     Represents one year of the competition
    /// </summary>
    public class KasiopeaCompetition : KasiopeaResolvableEntity<KasiopeaCompetition>
    {
        private const string CurrentCompetitionUrl = "/soutez/";

        public KasiopeaCompetition(string url, string name = null) : base(url) {
            Name = name;
        }

        public bool IsCurrent => Url == CurrentCompetitionUrl;

        /// <summary>
        ///     Name of the competition, usually the year it's organized in, e.g. 2015
        /// </summary>
        public string Name { get; }

        public IEnumerable<KasiopeaStage> Stages { get; private set; }

        public static KasiopeaCompetition FromYear(int year) {
            if (year == DateTime.Now.Year) return GetCurrentCompetition();
            return new KasiopeaCompetition($"/archiv/{year}/", year.ToString());
        }

        public static KasiopeaCompetition GetCurrentCompetition() {
            return new KasiopeaCompetition(CurrentCompetitionUrl);
        }

        public override async Task<KasiopeaCompetition> Resolve(KasiopeaInterface kInterface) {
            if (Url == null) throw new InvalidOperationException("No Competition Url specified");
            var res = await kInterface.DownloadStringAsync(Url);
            var doc = new HtmlDocument {OptionFixNestedTags = true};
            doc.LoadHtml(res);
            if (IsCurrent) {
                var title = doc.DocumentNode.SelectSingleNode("//div[@class='top-sld'][1]/div[1]/h2");
                var stage = new KasiopeaStage(title.InnerText.Trim(), Url, this);
                Stages = new[] {stage};
            } else {
                var uls = doc.DocumentNode.SelectNodes("(//li[@class='actual'])[1]/ul[1]/li");
                Stages = uls.Select(x => x.SelectSingleNode("a"))
                    .Select(x => new KasiopeaStage(x.InnerText.Trim(), x.GetAttributeValue("href", null), this));
            }
            // sets the Resolved property to true
            await base.Resolve(kInterface);
            return this;
        }
    }
}