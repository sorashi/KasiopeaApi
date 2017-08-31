using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KasiopeaApi
{
    public class KasiopeaStage : KasiopeaResolvableEntity<KasiopeaStage>
    {
        public KasiopeaStage(string name, string url, KasiopeaCompetition comp = null) : base(url) {
            Name = name;
            Competition = comp;
        }

        public KasiopeaCompetition Competition { get; }

        /// <summary>
        ///     The name of the stage, e.g. Domácí kolo
        /// </summary>
        public string Name { get; }

        public IEnumerable<KasiopeaTask> Tasks { get; private set; }

        public override async Task<KasiopeaStage> Resolve(KasiopeaInterface kInterface) {
            if (string.IsNullOrWhiteSpace(Url)) throw new InvalidOperationException("No Stage Url specified");
            var res = kInterface.DownloadStringAsync(Url);
            var doc = new HtmlDocument {OptionFixNestedTags = true};
            doc.LoadHtml(await res);
            if (!Competition.IsCurrent) {
                // skips first <li>, because it contains the link to results, may be needed to change in the future, if there is some small change on the site
                var tasks = doc.DocumentNode.SelectNodes("(//li[@class='actual'])[2]/ul/li[position()>1]");
                Tasks = tasks.Select(x => x.SelectSingleNode("a"))
                    .Select(x => {
                        var text = x.InnerHtml.Trim();
                        return new KasiopeaTask(text.Substring(0, text.IndexOf(':'))[0],
                            text.Substring(text.IndexOf(':') + 1), x.GetAttributeValue("href", null));
                    });
            } else {
                // Skips first two links - Introduction and Results
                var tasks = doc.DocumentNode.SelectNodes("//div[@class='sidebar']/ul[1]/li[position()>2]/a");
                Tasks = tasks.Select(x => {
                    var text = x.InnerHtml.Trim();
                    return new KasiopeaTask(text.Substring(0, text.IndexOf(':'))[0],
                        text.Substring(text.IndexOf(':') + 1).Trim(), x.GetAttributeValue("href", null), this);
                });
            }
            // sets the Resolved property to true
            await base.Resolve(kInterface);
            return this;
        }
    }
}