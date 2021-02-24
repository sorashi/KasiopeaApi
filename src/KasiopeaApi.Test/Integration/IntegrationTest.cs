using System;
using System.Diagnostics;
using System.Linq;
using KasiopeaApi.Rest;
using KasiopeaApi.Rest.Model;
using Xunit;

namespace KasiopeaApi.Test.Integration
{
    public class IntegrationTest
    {
        [SkippableFact]
        public async void GetInputSubmitOutput() {
            Skip.If(new[] {Constants.TestBase, Constants.TestEmail, Constants.TestPassword}.Any(x => x == null), "Environment variables are not set up.");
            KasiopeaAuthenticator.Test = true;
            var k = new KasiopeaInterface(Constants.TestEmail, Constants.TestPassword, Constants.TestBase);
            await k.SelectTaskAsync(2020, CourseKind.Home, 'A');
            var reader = await k.GetInputReaderAsync(Difficulty.Easy);
            var writer = k.GetOutputWriter();
            var t = int.Parse(reader.ReadLine());
            for (int i = 0; i < t; i++) {
                var n = reader.ReadLine().Split(' ').Select(int.Parse).ToArray();
                writer.WriteLine(n[0] - n[1] < n[2] ? "NE REKLAMU" : "REKLAMU");
            }

            var state = await k.PostOutputAsync();
            Assert.Equal(ApiAttemptState.Success, state);
        }
    }
}
