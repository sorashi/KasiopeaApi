using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace KasiopeaApi.Tests
{
    [TestFixture]
    public class KasiopeaArchiveTest
    {
        private KasiopeaInterface kasiopeaInterface;
        [Test]
        public async Task ArchiveTest() {
            var competition = await KasiopeaCompetition.FromYear(2015).Resolve(kasiopeaInterface);
            Assert.IsTrue(competition.Resolved);
            Assert.AreEqual("2015", competition.Name);
            Assert.IsNotEmpty(competition.Stages);
            var stage = await competition.Stages.First().Resolve(kasiopeaInterface);
            Assert.IsNotEmpty(stage.Tasks);
            Assert.IsFalse(string.IsNullOrWhiteSpace(stage.Name));
        }
        [SetUp]
        public async Task Setup() {
            kasiopeaInterface = new KasiopeaInterface();
            Assert.IsTrue(await kasiopeaInterface.Login(Credentials.Email, Credentials.Password));
        }
    }
}
